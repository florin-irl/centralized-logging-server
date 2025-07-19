package com.example.charlieservice.service;

import com.example.charlieservice.model.Phone;
import com.example.charlieservice.repository.PhoneRepository;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;

@Service
public class PhoneService {

    private final PhoneRepository repository;

    public PhoneService(PhoneRepository repository) {
        this.repository = repository;
    }

    public List<Phone> getAllPhones() {
        return repository.findAll();
    }

    public Optional<Phone> getPhoneById(Integer id) {
        return repository.findById(id);
    }

    public Phone addPhone(Phone phone) {
        return repository.save(phone);
    }

    public Phone updatePhone(Integer id, Phone phone) {
        phone.setPhoneId(id);
        return repository.save(phone);
    }

    public void deletePhone(Integer id) {
        repository.deleteById(id);
    }
}
