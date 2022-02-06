using System;


namespace CloudSeedApp {
    public class UtcNowProvider : INowProvider {

        public DateTimeOffset GetNowDateTimeOffset() {
            return DateTimeOffset.UtcNow;
        }
        
    }
}