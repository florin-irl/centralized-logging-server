package com.example.charlieservice.model;

import jakarta.persistence.*;

@Entity
@Table(name = "\"Phones\"") // Explicitly match the exact table name
public class Phone {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "\"PhoneId\"")  // Match exact column name
    private Integer phoneId;

    @Column(name = "\"PhoneNr\"", nullable = false, length = 20)
    private String phoneNr;

    public Phone() {}

    public Phone(String phoneNr) {
        this.phoneNr = phoneNr;
    }

    public Integer getPhoneId() {
        return phoneId;
    }

    public void setPhoneId(Integer phoneId) {
        this.phoneId = phoneId;
    }

    public String getPhoneNr() {
        return phoneNr;
    }

    public void setPhoneNr(String phoneNr) {
        this.phoneNr = phoneNr;
    }
}