CREATE DATABASE travel_company_db
    WITH 
    OWNER = postgres
    ENCODING = 'UTF8'
    LC_COLLATE = 'en_US.UTF-8'
    LC_CTYPE = 'en_US.UTF-8'
    TEMPLATE = template0;

\c travel_company_db

-- Employees table
CREATE TABLE employees (
    employee_id SERIAL PRIMARY KEY,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    hire_date DATE NOT NULL,
    is_active BOOLEAN DEFAULT TRUE
);

-- Tour Operators table
CREATE TABLE tour_operators (
    tour_operator_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    contact_email VARCHAR(100) UNIQUE NOT NULL,
    phone VARCHAR(20),
    address TEXT
);

-- Clients table
CREATE TABLE clients (
    client_id SERIAL PRIMARY KEY,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    phone VARCHAR(20),
    registration_date DATE NOT NULL
);

-- Countries table
CREATE TABLE countries (
    country_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE,
    code CHAR(2)
);

-- Populated Places table
CREATE TABLE populated_places (
    place_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    country_id INTEGER NOT NULL,
    FOREIGN KEY (country_id) REFERENCES countries(country_id) ON DELETE RESTRICT
);

-- Streets table
CREATE TABLE streets (
    street_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    place_id INTEGER NOT NULL,
    FOREIGN KEY (place_id) REFERENCES populated_places(place_id) ON DELETE RESTRICT
);

-- Hotels table
CREATE TABLE hotels (
    hotel_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    street_id INTEGER NOT NULL,
    phone VARCHAR(20),
    rating DECIMAL(3,1) CHECK (rating >= 0 AND rating <= 5),
    FOREIGN KEY (street_id) REFERENCES streets(street_id) ON DELETE RESTRICT
);

-- Routes table
CREATE TABLE routes (
    route_id SERIAL PRIMARY KEY,
    origin_place_id INTEGER NOT NULL,
    destination_place_id INTEGER NOT NULL,
    distance_km DECIMAL(10,2),
    duration_minutes INTEGER,
    FOREIGN KEY (origin_place_id) REFERENCES populated_places(place_id) ON DELETE RESTRICT,
    FOREIGN KEY (destination_place_id) REFERENCES populated_places(place_id) ON DELETE RESTRICT,
    CHECK (origin_place_id != destination_place_id)
);

-- Tourist Groups table
CREATE TABLE tourist_groups (
    group_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    tour_operator_id INTEGER NOT NULL,
    start_date DATE NOT NULL,
    end_date DATE,
    FOREIGN KEY (tour_operator_id) REFERENCES tour_operators(tour_operator_id) ON DELETE RESTRICT
);

-- Client-Group Junction table (many-to-many)
CREATE TABLE client_group (
    client_id INTEGER NOT NULL,
    group_id INTEGER NOT NULL,
    FOREIGN KEY (client_id) REFERENCES clients(client_id) ON DELETE CASCADE,
    FOREIGN KEY (group_id) REFERENCES tourist_groups(group_id) ON DELETE CASCADE,
    PRIMARY KEY (client_id, group_id)
);

-- Payments table
CREATE TABLE payments (
    payment_id SERIAL PRIMARY KEY,
    client_id INTEGER NOT NULL,
    amount DECIMAL(10,2) NOT NULL CHECK (amount >= 0),
    payment_date DATE NOT NULL,
    status VARCHAR(20) CHECK (status IN ('Pending', 'Completed', 'Failed')),
    FOREIGN KEY (client_id) REFERENCES clients(client_id) ON DELETE RESTRICT
);

-- Penalties table
CREATE TABLE penalties (
    penalty_id SERIAL PRIMARY KEY,
    client_id INTEGER NOT NULL,
    amount DECIMAL(10,2) NOT NULL CHECK (amount >= 0),
    reason TEXT NOT NULL,
    issue_date DATE NOT NULL,
    FOREIGN KEY (client_id) REFERENCES clients(client_id) ON DELETE RESTRICT
);

-- Reports table
CREATE TABLE reports (
    report_id SERIAL PRIMARY KEY,
    employee_id INTEGER NOT NULL,
    title VARCHAR(100) NOT NULL,
    content TEXT,
    created_date DATE NOT NULL,
    FOREIGN KEY (employee_id) REFERENCES employees(employee_id) ON DELETE RESTRICT
);

-- Indexes for performance
CREATE INDEX idx_tourist_groups_tour_operator_id ON tourist_groups(tour_operator_id);
CREATE INDEX idx_client_group_client_id ON client_group(client_id);
CREATE INDEX idx_payments_client_id ON payments(client_id);
CREATE INDEX idx_penalties_client_id ON penalties(client_id);
CREATE INDEX idx_reports_employee_id ON reports(employee_id);
