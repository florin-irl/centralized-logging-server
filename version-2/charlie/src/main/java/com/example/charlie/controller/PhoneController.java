package com.example.charlie.controller;

import com.example.charlie.model.Phone;
import com.example.charlie.repository.PhoneRepository;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;
import org.apache.logging.log4j.ThreadContext;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.UUID;

@RestController
@RequestMapping("/api/phones")
public class PhoneController {

    private static final Logger log = LogManager.getLogger(PhoneController.class);
    private final PhoneRepository phoneRepository;

    public PhoneController(PhoneRepository phoneRepository) {
        this.phoneRepository = phoneRepository;
    }

    private void enrichLogContext(String method, String url, int statusCode) {
        ThreadContext.put("service", "CharlieService");
        ThreadContext.put("traceid", UUID.randomUUID().toString());   // Replace with real trace if propagating
        ThreadContext.put("spanid", UUID.randomUUID().toString());
        ThreadContext.put("method", method);
        ThreadContext.put("statuscode", String.valueOf(statusCode));
        ThreadContext.put("url", url);
    }

    private void clearLogContext() {
        ThreadContext.clearAll();
    }

    @GetMapping
    public List<Phone> getAllPhones() {
        enrichLogContext("GET", "/api/phones", 200);
        log.info("HTTP GET | URL: /api/phones | Status: 200");
        clearLogContext();
        return phoneRepository.findAll();
    }

    @GetMapping("/{id}")
    public Phone getPhoneById(@PathVariable int id) {
        enrichLogContext("GET", "/api/phones/" + id, 200);
        log.info("HTTP GET | URL: /api/phones/{} | Status: 200", id);
        clearLogContext();
        return phoneRepository.findById(id);
    }

    @PostMapping
    public String createPhone(@RequestBody Phone phone) {
        phoneRepository.save(phone);
        enrichLogContext("POST", "/api/phones", 201);
        log.info("HTTP POST | URL: /api/phones | Status: 201 | Body: {}", phone);
        clearLogContext();
        return "Phone added successfully!";
    }

    @PutMapping("/{id}")
    public String updatePhone(@PathVariable int id, @RequestBody Phone phone) {
        phone.setPhoneId(id);
        phoneRepository.update(phone);
        enrichLogContext("PUT", "/api/phones/" + id, 200);
        log.info("HTTP PUT | URL: /api/phones/{} | Status: 200 | Body: {}", id, phone);
        clearLogContext();
        return "Phone updated successfully!";
    }

    @DeleteMapping("/{id}")
    public String deletePhone(@PathVariable int id) {
        phoneRepository.delete(id);
        enrichLogContext("DELETE", "/api/phones/" + id, 200);
        log.info("HTTP DELETE | URL: /api/phones/{} | Status: 200", id);
        clearLogContext();
        return "Phone deleted successfully!";
    }
}
