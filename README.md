Сделай скрипт для создание бд Postgresql Структура бд от продукта PostgreSQL 
CREATE TABLE public."UsersObjects" (
 user_id int4 NOT NULL,
 object_id int4 NOT NULL,
 can_create bool NOT NULL,
 can_read bool NOT NULL,
 can_update bool NOT NULL,
 can_delete bool NOT NULL,
 CONSTRAINT "PK_UsersObjects" PRIMARY KEY (user_id, object_id),
 CONSTRAINT "FK_UsersObjects_Objects" FOREIGN KEY (object_id) REFERENCES public."Objects"(id),
 CONSTRAINT "FK_UsersObjects_Users" FOREIGN KEY (user_id) REFERENCES public."Users"(id)
);
CREATE INDEX "IX_UsersObjects_object_id" ON public."UsersObjects" USING btree (object_id);
CREATE TABLE public."Objects" (
 id int4 DEFAULT nextval('objects_id_seq'::regclass) NOT NULL,
 "name" text NOT NULL,
 CONSTRAINT objects_pkey PRIMARY KEY (id)
);
CREATE TABLE public."Users" (
 id int4 DEFAULT nextval('users_id_seq'::regclass) NOT NULL,
 username text NOT NULL,
 "password" text NOT NULL,
 CONSTRAINT users_pkey PRIMARY KEY (id)
);
CREATE TABLE public."Hotel" (
 id int4 DEFAULT nextval('hotel_id_seq'::regclass) NOT NULL,
 "name" text NOT NULL,
 "class" text NOT NULL,
 CONSTRAINT hotel_pkey PRIMARY KEY (id)
);
CREATE TABLE public."RoutesPopulatedPlaces" (
 route_id int4 NOT NULL,
 populated_place_id int4 NOT NULL,
 hotel_id int4 NOT NULL,
 stay_start_datetime timestamp NOT NULL,
 stay_end_datetime timestamp NOT NULL,
 excursion_program text NOT NULL,
 CONSTRAINT "PK_RoutesPopulatedPlaces" PRIMARY KEY (route_id, populated_place_id),
 CONSTRAINT "FK_RoutesPopulatedPlaces_Hotel" FOREIGN KEY (hotel_id) REFERENCES public."Hotel"(id),
 CONSTRAINT "FK_RoutesPopulatedPlaces_PopulatedPlace" FOREIGN KEY (populated_place_id) REFERENCES public."PopulatedPlace"(id),
 CONSTRAINT "FK_RoutesPopulatedPlaces_Route" FOREIGN KEY (route_id) REFERENCES public."Route"(id)
);
CREATE INDEX "IX_RoutesPopulatedPlaces_hotel_id" ON public."RoutesPopulatedPlaces" USING btree (hotel_id);
CREATE INDEX "IX_RoutesPopulatedPlaces_populated_place_id" ON public."RoutesPopulatedPlaces" USING btree (populated_place_id);
CREATE TABLE public."PopulatedPlace" (
 id int4 DEFAULT nextval('populatedplace_id_seq'::regclass) NOT NULL,
 "name" text NOT NULL,
 country_id int4 NOT NULL,
 CONSTRAINT populatedplace_pkey PRIMARY KEY (id),
 CONSTRAINT "FK_PopulatedPlace_Country" FOREIGN KEY (country_id) REFERENCES public."Country"(id)
);
CREATE INDEX "IX_PopulatedPlace_country_id" ON public."PopulatedPlace" USING btree (country_id);
CREATE TABLE public."Country" (
 id int4 DEFAULT nextval('country_id_seq'::regclass) NOT NULL,
 "name" text NOT NULL,
 CONSTRAINT country_pkey PRIMARY KEY (id)
);
CREATE TABLE public."Route" (
 id int4 DEFAULT nextval('route_id_seq'::regclass) NOT NULL,
 "name" text NOT NULL,
 start_datetime timestamp NOT NULL,
 end_datetime timestamp NOT NULL,
 "cost" numeric(18, 2) NOT NULL,
 country_id int4 NULL,
 "TourOperatorId" int4 NULL,
 CONSTRAINT route_pkey PRIMARY KEY (id),
 CONSTRAINT "FK_Route_Country" FOREIGN KEY (country_id) REFERENCES public."Country"(id) ON DELETE SET NULL,
 CONSTRAINT "FK_Route_TourOperator_TourOperatorId" FOREIGN KEY ("TourOperatorId") REFERENCES public."TourOperator"(id)
);
CREATE INDEX "IX_Route_TourOperatorId" ON public."Route" USING btree ("TourOperatorId");
CREATE INDEX "IX_Route_country_id" ON public."Route" USING btree (country_id);
CREATE TABLE public."TourOperator" (
 id int4 DEFAULT nextval('touroperator_id_seq'::regclass) NOT NULL,
 "name" text NOT NULL,
 contact_info text NOT NULL,
 address text NOT NULL,
 CONSTRAINT touroperator_pkey PRIMARY KEY (id)
);
CREATE TABLE public."TouristGroup" (
 id int4 DEFAULT nextval('touristgroup_id_seq'::regclass) NOT NULL,
 "name" text NOT NULL,
 tour_guide_id int4 NOT NULL,
 route_id int4 NOT NULL,
 CONSTRAINT touristgroup_pkey PRIMARY KEY (id),
 CONSTRAINT "FK_TouristGroup_Route" FOREIGN KEY (route_id) REFERENCES public."Route"(id),CONSTRAINT "FK_TouristGroup_TourGuide" FOREIGN KEY (tour_guide_id) REFERENCES public."TourGuide"(id)
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
CREATE TABLE public."Street" (
 id int4 DEFAULT nextval('street_id_seq'::regclass) NOT NULL,
 "name" text NOT NULL,
 CONSTRAINT street_pkey PRIMARY KEY (id)
);