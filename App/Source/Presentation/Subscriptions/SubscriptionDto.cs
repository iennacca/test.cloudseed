using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CloudSeedApp;

public class SubscriptionDto
{ 
    [JsonProperty("productId")]
    public string ProductId { get; set; }

    [JsonProperty("expirationTimeStamp")]
    public DateTimeOffset ExpirationTimestamp { get; set; }
}