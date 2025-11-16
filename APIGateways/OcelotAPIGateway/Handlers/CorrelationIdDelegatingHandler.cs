using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public class CorrelationIdDelegatingHandler : DelegatingHandler
{
    private const string HeaderName = "X-Correlation-Id";

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // اگر از upstream هدر ارسال شده بود، نگه دار؛ در غیر این صورت مقدار جدید تولید کن
        if (!request.Headers.Contains(HeaderName))
        {
            // try to get from current HttpContext if available
            var ctx = request.Properties.ContainsKey("HttpContext") ? request.Properties["HttpContext"] : null;
            // but simplest: generate new guid
            request.Headers.Add(HeaderName, System.Guid.NewGuid().ToString());
        }

        // همچنین می‌توانید headerهای دیگری مثل Authorization را عبور دهید (پیش فرض Ocelot آنها را می‌گذارد)
        return base.SendAsync(request, cancellationToken);
    }
}
