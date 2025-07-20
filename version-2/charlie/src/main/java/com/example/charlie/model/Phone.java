package com.example.charlie.model;

public class Phone {
    private int PhoneId;
    private String PhoneNr;

    public Phone() {}
    public Phone(int phoneId, String phoneNr) {
        this.PhoneId = phoneId;
        this.PhoneNr = phoneNr;
    }

    public int getPhoneId() { return PhoneId; }
    public void setPhoneId(int phoneId) { this.PhoneId = phoneId; }

    public String getPhoneNr() { return PhoneNr; }
    public void setPhoneNr(String phoneNr) { this.PhoneNr = phoneNr; }
}
