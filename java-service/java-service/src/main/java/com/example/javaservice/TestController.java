package com.example.javaservice;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class TestController {

    private static final Logger logger = LogManager.getLogger(TestController.class);

    @GetMapping("/test")
    public String testLogging() {
        logger.info("Test log message from Spring Boot Java service.");
        return "Log sent!";
    }
}
