package com.example.charlieservice.controller;


import com.example.charlieservice.model.Phone;
import com.example.charlieservice.service.PhoneService;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/phones")
public class PhoneController {

    private final PhoneService service;

    public PhoneController(PhoneService service) {
        this.service = service;
    }

    @GetMapping
    public List<Phone> getAllPhones() {
        return service.getAllPhones();
    }

    @GetMapping("/{id}")
    public ResponseEntity<Phone> getPhoneById(@PathVariable Integer id) {
        return service.getPhoneById(id)
                .map(ResponseEntity::ok)
                .orElse(ResponseEntity.notFound().build());
    }

    @PostMapping
    public ResponseEntity<Phone> addPhone(@RequestBody Phone phone) {
        return ResponseEntity.ok(service.addPhone(phone));
    }

    @PutMapping("/{id}")
    public ResponseEntity<Phone> updatePhone(@PathVariable Integer id, @RequestBody Phone phone) {
        if (!service.getPhoneById(id).isPresent()) {
            return ResponseEntity.notFound().build();
        }
        return ResponseEntity.ok(service.updatePhone(id, phone));
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<Void> deletePhone(@PathVariable Integer id) {
        if (!service.getPhoneById(id).isPresent()) {
            return ResponseEntity.notFound().build();
        }
        service.deletePhone(id);
        return ResponseEntity.noContent().build();
    }
}