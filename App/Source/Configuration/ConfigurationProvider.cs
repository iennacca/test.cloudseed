using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;


namespace CloudSeedApp {
    /*
        * Central entry point for Environment Variable Configuration
    */
    public class ConfigurationProvider {
        public string ACCESS_TOKEN_COOKIE_NAME => this.TryGetEnvironmentVariable(nameof(ACCESS_TOKEN_COOKIE_NAME));
        public string DATABASE_HOST => this.TryGetEnvironmentVariable(nameof(DATABASE_HOST));
        public string DATABASE_NAME => this.TryGetEnvironmentVariable(nameof(DATABASE_NAME));
        public string DATABASE_PASSWORD => this.TryGetEnvironmentVariable(nameof(DATABASE_PASSWORD));
        public string DATABASE_USER => this.TryGetEnvironmentVariable(nameof(DATABASE_USER));
        public string EMAIL_FROM_ADDRESS => this.TryGetEnvironmentVariable(nameof(EMAIL_FROM_ADDRESS));
        public string JWT_SIGNING_KEY => this.TryGetEnvironmentVariable(nameof(JWT_SIGNING_KEY));
        public string POSTMARK_API_KEY => this.TryGetEnvironmentVariable(nameof(POSTMARK_API_KEY));
        public string STRIPE_API_KEY => this.TryGetEnvironmentVariable(nameof(STRIPE_API_KEY));
        public string STRIPE_WEBHOOK_SECRET => this.TryGetEnvironmentVariable(nameof(STRIPE_WEBHOOK_SECRET));
        public string WEB_BASE_URL => this.TryGetEnvironmentVariable(nameof(WEB_BASE_URL));

        public bool IsDevelopmentEnvironment => this._hostEnvironment.IsDevelopment();

        private IConfiguration _configuration;
        private IWebHostEnvironment _hostEnvironment;
        private Dictionary<string, string> _environmentVariableToValueLookup;
    
        public ConfigurationProvider(
            IConfiguration configuration,
            IWebHostEnvironment hostEnvironment
        ) {
            this._configuration = configuration;
            this._hostEnvironment = hostEnvironment;

            this._environmentVariableToValueLookup = new Dictionary<string, string>();
        }

        private string TryGetEnvironmentVariable(string variableName) {
            if(!this._environmentVariableToValueLookup.ContainsKey(variableName)) {
                var value = this._configuration[variableName];

                if(value is null) {
                    throw new InvalidOperationException($"Attempted to retrieve EnvironmentVariable {variableName} but got null.");
                }

                this._environmentVariableToValueLookup[variableName] = value;
            }

            return this._environmentVariableToValueLookup[variableName];
        }
    }
}