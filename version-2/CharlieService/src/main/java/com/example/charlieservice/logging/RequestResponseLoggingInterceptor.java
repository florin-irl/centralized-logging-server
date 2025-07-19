package com.example.charlieservice.logging;

import com.fasterxml.jackson.databind.ObjectMapper;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;
import org.springframework.stereotype.Component;
import org.springframework.web.servlet.HandlerInterceptor;

import java.util.HashMap;
import java.util.Map;

@Component
public class RequestResponseLoggingInterceptor implements HandlerInterceptor {

    private static final Logger logger = LogManager.getLogger(RequestResponseLoggingInterceptor.class);
    private final ObjectMapper objectMapper = new ObjectMapper();

    // Store start time before controller execution
    @Override
    public boolean preHandle(HttpServletRequest request, HttpServletResponse response, Object handler) {
        request.setAttribute("startTime", System.currentTimeMillis());
        return true;
    }

    // Log after controller execution
    @Override
    public void afterCompletion(HttpServletRequest request, HttpServletResponse response, Object handler, Exception ex) {
        long startTime = (long) request.getAttribute("startTime");
        long duration = System.currentTimeMillis() - startTime;

        Map<String, Object> logData = new HashMap<>();
        logData.put("operation", request.getMethod().equals("POST") ? "CREATE"
                : request.getMethod().equals("GET") ? "READ"
                : request.getMethod().equals("PUT") ? "UPDATE"
                : request.getMethod().equals("DELETE") ? "DELETE"
                : "OTHER");
        logData.put("endpoint", request.getRequestURI());
        logData.put("httpMethod", request.getMethod());
        logData.put("statusCode", response.getStatus());
        logData.put("executionTimeMs", duration);

        if (ex != null) {
            logData.put("error", ex.getMessage());
        }

        // Log as JSON
        try {
            logger.info(objectMapper.writeValueAsString(logData));
        } catch (Exception e) {
            logger.error("Failed to log structured data", e);
        }
    }
}
