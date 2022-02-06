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
    [Table("roles")]
    public class ApplicationRole {
        [Column("id")]
        [Key]
        public Guid Id { get; set; }

        [Column("name")]
        public string Name { get; set; }
    }
}