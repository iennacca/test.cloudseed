using System;


namespace CloudSeedApp {
    public interface INowProvider {
        public DateTimeOffset GetNowDateTimeOffset();
    }
}