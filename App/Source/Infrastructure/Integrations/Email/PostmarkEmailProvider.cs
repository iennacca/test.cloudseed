using System;
using System.Threading.Tasks;
using PostmarkDotNet;

namespace CloudSeedApp
{
    public class PostmarkEmailProvider : IEmailProvider {

        private ConfigurationProvider _configuration;

        public PostmarkEmailProvider(ConfigurationProvider configuration) {
            this._configuration = configuration;
        }

        public async Task<bool> TrySendEmailAsync(
            string toEmailAddress,
            string fromEmailAddress,
            string subject,
            string textBody
        ) {
            var message = new PostmarkMessage()
            {
                To = toEmailAddress,
                From = fromEmailAddress,
                Subject = subject,
                TextBody = textBody,
                MessageStream = "outbound"
            };

            var client = new PostmarkClient(
                this._configuration.POSTMARK_API_KEY
            );
            var sendResult = await client.SendMessageAsync(message);

            if(sendResult.Status == PostmarkStatus.Success) {
                return true;
            } else {
                throw new Exception(
                    sendResult.Message
                );
            }
        }
    }
}