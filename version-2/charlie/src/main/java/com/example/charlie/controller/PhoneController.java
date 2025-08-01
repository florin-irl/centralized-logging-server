package com.example.charlie.controller;

import com.example.charlie.model.Phone;
import com.example.charlie.repository.PhoneRepository;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;
import org.springframework.web.bind.annotation.*;

import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

@RestController
@RequestMapping("/api/phones")
public class PhoneController {

    // The logger is still needed for writing messages.
    private static final Logger log = LogManager.getLogger(PhoneController.class);
    private final PhoneRepository phoneRepository;

    public PhoneController(PhoneRepository phoneRepository) {
        this.phoneRepository = phoneRepository;
    }

    // The enrichLogContext and clearLogContext methods have been completely removed.

    @GetMapping
    public List<Phone> getAllPhones() {
        // The TraceIdFilter has already added the trace.id to the context.
        // The EcsLayout will automatically add service.name, http.request.method, etc.
        log.info("Processing request to get all phones.");
        return phoneRepository.findAll();
    }

    @GetMapping("/{id}")
    public Phone getPhoneById(@PathVariable int id) {
        log.info("Processing request to get phone with id: {}", id);
        return phoneRepository.findById(id);
    }

    @PostMapping
    public String createPhone(@RequestBody Phone phone) {
        log.info("Processing request to create a new phone: {}", phone);
        phoneRepository.save(phone);
        return "Phone added successfully!";
    }

    @GetMapping("/health")
    public Map<String, Object> healthCheck() {
        Map<String, Object> result = new HashMap<>();
        try {
            boolean dbHealthy = !phoneRepository.findAll().isEmpty() || true;

            result.put("service", "CharlieService");
            result.put("status", dbHealthy ? "Healthy" : "Degraded");
            result.put("database", dbHealthy ? "Connected" : "Not Reachable");
            result.put("timestamp", new Date());

            log.info("Health check successful.");
            return result;
        } catch (Exception ex) {
            result.put("service", "CharlieService");
            result.put("status", "Unhealthy");
            result.put("database", "Not Reachable");
            result.put("timestamp", new Date());

            // The EcsLayout will automatically format the exception correctly.
            log.error("Error during health check.", ex);
            return result;
        }
    }

    @PutMapping("/{id}")
    public String updatePhone(@PathVariable int id, @RequestBody Phone phone) {
        phone.setPhoneId(id);
        log.info("Processing request to update phone with id: {}", id);
        phoneRepository.update(phone);
        return "Phone updated successfully!";
    }

    @DeleteMapping("/{id}")
    public String deletePhone(@PathVariable int id) {
        log.info("Processing request to delete phone with id: {}", id);
        phoneRepository.delete(id);
        return "Phone deleted successfully!";
    }
}