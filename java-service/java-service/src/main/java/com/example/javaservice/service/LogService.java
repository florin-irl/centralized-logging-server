package com.example.javaservice.service;

import com.example.javaservice.model.LogEntry;
import com.example.javaservice.repository.LogEntryRepository;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;
import org.springframework.stereotype.Service;

import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;

@Service
public class LogService {

    private static final Logger logger = LogManager.getLogger(LogService.class);

    private final LogEntryRepository repository;

    public LogService(LogEntryRepository repository) {
        this.repository = repository;
    }

    public List<LogEntry> getAll() {
        logger.info("Fetching all log entries");
        return repository.findAll();
    }

    public Optional<LogEntry> getById(Long id) {
        logger.info("Fetching log entry with ID {}", id);
        return repository.findById(id);
    }

    public LogEntry create(LogEntry entry) {
        entry.setTimestamp(LocalDateTime.now());
        logger.info("Creating new log entry: {}", entry.getMessage());
        return repository.save(entry);
    }

    public void delete(Long id) {
        logger.info("Deleting log entry with ID {}", id);
        repository.deleteById(id);
    }
}
