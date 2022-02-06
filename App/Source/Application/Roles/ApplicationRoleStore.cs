using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CloudSeedApp;
using Newtonsoft.Json;

namespace CloudSeedApp {
    /*
        * ApplicationRoleStore for use by Microsoft Identity
    */

    public class ApplicationRoleStore : IRoleStore<ApplicationRole> {

        private CloudSeedAppDatabaseContext _dbContext;

        public ApplicationRoleStore(CloudSeedAppDatabaseContext dbContext) {
            this._dbContext = dbContext;
        }

        public void Dispose()
        {
            // Intentional no-op to fulfill IRoleStore contract
        }

        public async Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken = default(CancellationToken)) {
            cancellationToken.ThrowIfCancellationRequested();

            this._dbContext
                .Roles
                .Add(role);
            await this._dbContext.SaveChangesAsync();
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            this._dbContext
                .MarkDataAsModified(role);
            await this._dbContext.SaveChangesAsync();
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationRole role, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            this._dbContext
                .Roles
                .Remove(role);
            await this._dbContext.SaveChangesAsync();
            return IdentityResult.Success;
        }

        #region roleid
        public async Task<ApplicationRole> FindByIdAsync(string roleId, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if(!Guid.TryParse(roleId, out var guidId)) {
                throw new ArgumentException($"Could not parse as guid: {roleId}");
            }

            return await this._dbContext
                .Roles
                .SingleAsync(r => r.Id == guidId);
        }

        public async Task<string> GetRoleIdAsync(ApplicationRole role, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            return role.Id.ToString();
        }
        #endregion

        #region rolename
        public async Task<ApplicationRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public async Task<string> GetNormalizedRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public async Task<string> GetRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public async Task SetNormalizedRoleNameAsync(ApplicationRole role, string normalizedName, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task SetRoleNameAsync(ApplicationRole role, string roleName, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }
        #endregion
    }
}