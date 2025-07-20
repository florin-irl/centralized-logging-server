package com.example.charlie.controller;

import com.example.charlie.model.Phone;
import com.example.charlie.repository.PhoneRepository;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/phones")
public class PhoneController {

    private final PhoneRepository phoneRepository;

    public PhoneController(PhoneRepository phoneRepository) {
        this.phoneRepository = phoneRepository;
    }

    @GetMapping
    public List<Phone> getAllPhones() {
        return phoneRepository.findAll();
    }

    @GetMapping("/{id}")
    public Phone getPhoneById(@PathVariable int id) {
        return phoneRepository.findById(id);
    }

    @PostMapping
    public String createPhone(@RequestBody Phone phone) {
        phoneRepository.save(phone);
        return "Phone added successfully!";
    }

    @PutMapping("/{id}")
    public String updatePhone(@PathVariable int id, @RequestBody Phone phone) {
        phone.setPhoneId(id);
        phoneRepository.update(phone);
        return "Phone updated successfully!";
    }

    @DeleteMapping("/{id}")
    public String deletePhone(@PathVariable int id) {
        phoneRepository.delete(id);
        return "Phone deleted successfully!";
    }
}
