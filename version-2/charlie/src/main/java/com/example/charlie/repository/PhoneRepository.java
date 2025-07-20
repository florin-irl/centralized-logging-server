package com.example.charlie.repository;

import com.example.charlie.model.Phone;
import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.jdbc.core.RowMapper;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public class PhoneRepository {

    private final JdbcTemplate jdbcTemplate;

    public PhoneRepository(JdbcTemplate jdbcTemplate) {
        this.jdbcTemplate = jdbcTemplate;
    }

    private final RowMapper<Phone> phoneRowMapper = (rs, rowNum) ->
            new Phone(rs.getInt("PhoneId"), rs.getString("PhoneNr"));

    public List<Phone> findAll() {
        return jdbcTemplate.query("SELECT \"PhoneId\", \"PhoneNr\" FROM \"Phones\"", phoneRowMapper);
    }

    public Phone findById(int id) {
        return jdbcTemplate.queryForObject(
                "SELECT \"PhoneId\", \"PhoneNr\" FROM \"Phones\" WHERE \"PhoneId\" = ?",
                phoneRowMapper,
                id
        );
    }

    public int save(Phone phone) {
        return jdbcTemplate.update(
                "INSERT INTO \"Phones\" (\"PhoneNr\") VALUES (?)",
                phone.getPhoneNr()
        );
    }

    public int update(Phone phone) {
        return jdbcTemplate.update(
                "UPDATE \"Phones\" SET \"PhoneNr\" = ? WHERE \"PhoneId\" = ?",
                phone.getPhoneNr(),
                phone.getPhoneId()
        );
    }

    public int delete(int id) {
        return jdbcTemplate.update("DELETE FROM \"Phones\" WHERE \"PhoneId\" = ?", id);
    }
}
