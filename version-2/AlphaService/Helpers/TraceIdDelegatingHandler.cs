using System.Diagnostics;

namespace AlphaService.Helpers
{
    public class TraceIdDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TraceIdDelegatingHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // --> THE FIX, PART 2: Read the ID directly from HttpContext.Items
            if (_httpContextAccessor.HttpContext?.Items["TraceId"] is string traceId)
            {
                // Add the trace ID as a header to the OUTGOING request
                request.Headers.Add("X-Trace-Id", traceId);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
