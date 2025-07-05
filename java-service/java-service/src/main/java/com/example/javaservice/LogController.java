package com.example.javaservice.controller;

import com.example.javaservice.model.LogEntry;
import com.example.javaservice.service.LogService;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/logs")
public class LogController {

    private final LogService service;

    public LogController(LogService service) {
        this.service = service;
    }

    @GetMapping
    public List<LogEntry> getAll() {
        return service.getAll();
    }

    @GetMapping("/{id}")
    public ResponseEntity<LogEntry> getById(@PathVariable Long id) {
        return service.getById(id)
                .map(ResponseEntity::ok)
                .orElse(ResponseEntity.notFound().build());
    }

    @PostMapping
    public LogEntry create(@RequestBody LogEntry entry) {
        return service.create(entry);
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<Void> delete(@PathVariable Long id) {
        service.delete(id);
        return ResponseEntity.noContent().build();
    }
}
