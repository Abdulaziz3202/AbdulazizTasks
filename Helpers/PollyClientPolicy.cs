using Polly.Retry;
using Polly;

namespace MVCRESTAPI.Helpers
{
    public class PollyClientPolicy
    {
        public AsyncRetryPolicy<HttpResponseMessage> ImmediateHttpRetry { get; }
        public AsyncRetryPolicy<HttpResponseMessage> LinearHttpRetry { get; }

        public AsyncRetryPolicy<HttpResponseMessage> ExponentialHttpRetry { get; }

        public PollyClientPolicy()
        {
            // If fails do another request immediately and keep trying for 5 times as maximum. 
            ImmediateHttpRetry = Policy.HandleResult<HttpResponseMessage>(res => !res.IsSuccessStatusCode).RetryAsync(5);

            // If fails do another request after 3 seconds and keep trying for 5 times as maximum. 
            LinearHttpRetry = Policy.HandleResult<HttpResponseMessage>(res => !res.IsSuccessStatusCode).WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(3));

            // If fails do another request after 1, 3 , 7 , 15 ... seconds and keep trying for 5 times as maximum.
            ExponentialHttpRetry = Policy.HandleResult<HttpResponseMessage>(res => !res.IsSuccessStatusCode).WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

    }
}
