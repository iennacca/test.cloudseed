using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace CloudSeedApp
{
    [ApiController]
    public class SentinelController : ControllerBase
    {
        private readonly ILogger<SentinelController> _logger;
        private CloudSeedAppDatabaseContext _linetimesDatabaseContext;

        private readonly IMediator _mediator;

        public SentinelController(
            ILogger<SentinelController> logger,
            CloudSeedAppDatabaseContext linetimesDatabaseContext,
            IMediator mediator)
        {
            _logger = logger;
            this._linetimesDatabaseContext = linetimesDatabaseContext;
            this._mediator = mediator;
        }

        [HttpGet]
        [Route("/sentinels")]
        [AllowAnonymous]
        public async Task<IEnumerable<Sentinel>> GetAsync()
        {
            this._logger.LogDebug("SentinelController hit");

            await this._mediator.Send(new CreateSentinelCommand() { Name = Guid.NewGuid().ToString() });
            return await this._mediator.Send(new GetSentinelsQuery());
        }

        [HttpGet]
        [Route("/sentinels/authorize")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IEnumerable<Sentinel>> GetAuthorizedAsync() {
            return await GetAsync();
        }
    }
}
