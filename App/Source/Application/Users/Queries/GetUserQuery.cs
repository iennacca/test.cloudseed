using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CloudSeedApp {
    public class GetUserQuery : IRequest<User?> { 
        public Guid UserId { get; set; }
    }

    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, User?> {
        private readonly UserDataProvider _userProvider;

        public GetUserQueryHandler(
            UserDataProvider userProvider
        ) {
            this._userProvider = userProvider;
        }

        public async Task<User?> Handle(GetUserQuery request, CancellationToken cancellationToken) {
            
            var user = await this._userProvider
                .TryGetUserAsync(request.UserId);
            return user;
        }
    }
}