using System;
using System.Threading.Tasks;

namespace CloudSeedApp
{
    public interface IEmailProvider {

        Task<bool> TrySendEmailAsync(
            string toEmailAddress,
            string fromEmailAddress,
            string subject,
            string textBody
        );
    }
}