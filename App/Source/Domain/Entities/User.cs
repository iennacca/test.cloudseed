using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CloudSeedApp;
using Newtonsoft.Json;

namespace CloudSeedApp {
    /*
        * Represents a user in our database
    */
    [Table("users")]
    public class User {
        [Column("id")]
        [Key]
        public Guid Id { get; }

        [Column("email")]
        public string Email { get; set; }

        [Column("data", TypeName = "jsonb")]
        // [JsonConverter(typeof(NullableTypeJsonConverter))]
        public UserData Data { get; set; }

        public User(
            string email,
            UserData data
        ) {
            this.Email = email;
            this.Data = data;
        }

        public class UserData {
            [JsonProperty("github_username")]
            [JsonConverter(typeof(NullableTypeJsonConverter))]
            public string? GitHubUsername { get; set; }

            [JsonProperty("gitlab_username")]
            [JsonConverter(typeof(NullableTypeJsonConverter))]
            public string? GitLabUsername { get; set; }

            [JsonProperty("stripe_customer_id")]
            [JsonConverter(typeof(NullableTypeJsonConverter))]
            public string? StripeCustomerId { get; set; }
        } 
    }
}