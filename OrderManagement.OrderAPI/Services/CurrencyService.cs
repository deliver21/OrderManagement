﻿using Newtonsoft.Json;
using OrderManagement.OrderAPI.Models;
using OrderManagement.OrderAPI.Services.IServices;
using OrderManagement.OrderAPI.Utilities;
using System;

namespace OrderManagement.OrderAPI.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public CurrencyService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }
        public async Task<int> CalculatePriority(Order order)
        {
            try
            {
                if (order.Status == SD.StatusPending)
                {
                    if (order.TotalAmountInBaseCurrency > 1)
                    {
                        // Calculate the total hours since the order was created
                        var hoursSinceCreation = (DateTime.Now - order.OrderDate).TotalHours;

                        // Add one point per hour since order creation
                        order.Priority = (int)((int)order.TotalAmountInBaseCurrency + hoursSinceCreation);
                    }
                    else
                    {
                        order.Priority = (int)(order.TotalAmount * (1 / await GetExchangeRate(order.Currency)));
                    }
                }
            }
            catch (Exception)
            {
                return 0;
            }
            return order.Priority;
        }
        public async Task<decimal?> GetExchangeRate(string currencyCode)
        {
            // Call the external API to get the exchange rate
            var apiKey = _configuration["ExchangeRateApi:CurrencyApiKey"]; // API key from config
            var baseUrl = _configuration["ExchangeRateApi:CurrencyApiBaseUrl"];// Base URL from config
            var response = await _httpClient.GetAsync($"{baseUrl}?access_key={apiKey}&symbols={currencyCode},USD,BYN,PLN,RUB,CDF,MXN&format=1");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var exchangeData = JsonConvert.DeserializeObject<CurrencyApiResponse>(result);

                // Assuming the API returns rates in a dictionary format like { "USD": 1.0, "EUR": 0.85, ... }
                decimal? rate;
                rate = exchangeData.rates.First(u => u.Key == currencyCode).Value;
                if (rate != null)
                {
                    return rate;
                }
            }
            return null; // Return null if the API call fails or the currency is not found
        }
        public class CurrencyApiResponse
        {
            [JsonProperty("rates")]  // Ensure it matches the actual response property name
            public Dictionary<string, decimal> rates { get; set; }
        }

    }
}
