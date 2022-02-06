using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CloudSeedApp;

public class OrderManagementDto
{ 
    [JsonProperty("url")]
    public string Url { get; set; }
}