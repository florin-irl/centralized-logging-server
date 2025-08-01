package com.example.charlie.config;

import jakarta.servlet.*;
import jakarta.servlet.http.HttpServletRequest;
import org.slf4j.MDC;
import org.springframework.stereotype.Component;

import java.io.IOException;
import java.util.UUID;

/**
 * A servlet filter that intercepts every incoming request to add a trace ID
 * to the logging context (MDC).
 *
 * It looks for an "X-Trace-Id" header. If found, it uses that value.
 * If not found, it generates a new unique ID for the request.
 *
 * The EcsLayout is automatically configured to read "trace.id" from the MDC.
 */
@Component
public class TraceIdFilter implements Filter {

    private static final String TRACE_ID_HEADER = "X-Trace-Id";
    private static final String TRACE_ID_MDC_KEY = "trace.id";

    @Override
    public void doFilter(ServletRequest request, ServletResponse response, FilterChain chain)
            throws IOException, ServletException {

        HttpServletRequest httpRequest = (HttpServletRequest) request;
        String traceId = httpRequest.getHeader(TRACE_ID_HEADER);

        // If the header is missing from the incoming request, generate a new trace ID.
        if (traceId == null || traceId.isEmpty()) {
            traceId = UUID.randomUUID().toString();
        }

        // Put the trace ID into the Mapped Diagnostic Context (MDC).
        MDC.put(TRACE_ID_MDC_KEY, traceId);
        try {
            // Continue the request chain. All logging from this point on will have the trace.id.
            chain.doFilter(request, response);
        } finally {
            // CRITICAL: Always clean up the MDC after the request is complete
            // to prevent the trace ID from leaking to other requests on the same thread.
            MDC.remove(TRACE_ID_MDC_KEY);
        }
    }
}