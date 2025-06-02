CREATE TABLE public."Street" (
    id int4 DEFAULT nextval('street_id_seq'::regclass) NOT NULL,
    "name" text NOT NULL,
    CONSTRAINT street_pkey PRIMARY KEY (id)
);

CREATE TABLE public."TourGuide" (
    id int4 DEFAULT nextval('tourguide_id_seq'::regclass) NOT NULL,
    first_name text NOT NULL,
    last_name text NOT NULL,
    patronymic text NULL,
    street_id int4 NOT NULL,
    birthdate date NOT NULL,
    salary numeric(18, 2) NULL,
    is_fired bool NOT NULL,
    fired_date date NULL,
    CONSTRAINT tourguide_pkey PRIMARY KEY (id),
    CONSTRAINT "FK_TourGuide_Street" FOREIGN KEY (street_id) REFERENCES public."Street"(id)
);

CREATE INDEX "IX_TourGuide_street_id" ON public."TourGuide" USING btree (street_id);

CREATE TABLE public."Client" (
    id int4 DEFAULT nextval('client_id_seq'::regclass) NOT NULL,
    first_name text NOT NULL,
    last_name text NOT NULL,
    patronymic text NULL,
    birthdate date NOT NULL,
    street_id int4 NOT NULL,
    passport_series text NOT NULL,
    passport_number text NOT NULL,
    passport_issue_date date NOT NULL,
    passport_issuing_authority text NOT NULL,
    photograph bytea NULL,
    CONSTRAINT client_pkey PRIMARY KEY (id),
    CONSTRAINT "FK_Client_Street" FOREIGN KEY (street_id) REFERENCES public."Street"(id)
);

CREATE INDEX "IX_Client_street_id" ON public."Client" USING btree (street_id);

CREATE TABLE public."Payments" (
    payment_id int4 DEFAULT nextval('payments_payment_id_seq'::regclass) NOT NULL,
    route_id int4 NOT NULL,
    "ClientId" int4 NOT NULL,
    "Amount" numeric(18, 2) NOT NULL,
    payment_date timestamp NOT NULL,
    payment_method text NOT NULL,
    status text NOT NULL,
    "comment" text NOT NULL,
    CONSTRAINT payments_pkey PRIMARY KEY (payment_id),
    CONSTRAINT "FK_Payment_Client" FOREIGN KEY ("ClientId") REFERENCES public."Client"(id),
    CONSTRAINT "FK_Payment_Route" FOREIGN KEY (route_id) REFERENCES public."Route"(id)
);

CREATE INDEX "IX_Payments_ClientId" ON public."Payments" USING btree ("ClientId");
CREATE INDEX "IX_Payments_route_id" ON public."Payments" USING btree (route_id);

CREATE TABLE public."TouristGroup" (
    id int4 DEFAULT nextval('touristgroup_id_seq'::regclass) NOT NULL,
    route_id int4 NOT NULL,
    tour_guide_id int4 NOT NULL,
    CONSTRAINT "PK_TouristGroup" PRIMARY KEY (id),
    CONSTRAINT "FK_TouristGroup_TourGuide" FOREIGN KEY (tour_guide_id) REFERENCES public."TourGuide"(id)
);

CREATE INDEX "IX_TouristGroup_route_id" ON public."TouristGroup" USING btree (route_id);
CREATE INDEX "IX_TouristGroup_tour_guide_id" ON public."TouristGroup" USING btree (tour_guide_id);

CREATE TABLE public."ClientsTouristGroups" (
    client_id int4 NOT NULL,
    tourist_group_id int4 NOT NULL,
    CONSTRAINT "PK_ClientsTouristGroups" PRIMARY KEY (client_id, tourist_group_id),
    CONSTRAINT "FK_ClientsTouristGroups_Client" FOREIGN KEY (client_id) REFERENCES public."Client"(id),
    CONSTRAINT "FK_ClientsTouristGroups_TouristGroup" FOREIGN KEY (tourist_group_id) REFERENCES public."TouristGroup"(id)
);

CREATE INDEX "IX_ClientsTouristGroups_tourist_group_id" ON public."ClientsTouristGroups" USING btree (tourist_group_id);