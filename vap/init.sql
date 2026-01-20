CREATE DATABASE SkolaDB;
GO
USE SkolaDB;
GO


CREATE TABLE predmety(
	id uniqueidentifier NOT NULL primary key,
	nazev nvarchar(100) NOT NULL,
	zkratka nchar(3) NOT NULL,
)
GO

CREATE TABLE studenti(
	id uniqueidentifier NOT NULL primary key,
	jmeno nvarchar(100) NOT NULL,
	prijmeni nvarchar(100) NOT NULL,
	datum_narozeni date NULL,
	isic nvarchar(20) NULL,
)
GO

CREATE TABLE znamky(
	id uniqueidentifier NOT NULL primary key,
	hodnota decimal(2, 1) NOT NULL,
	vaha decimal(2, 0) NOT NULL,
	idPredmety uniqueidentifier NOT NULL,
	poznamka nvarchar(max) NULL,
	idStudenti uniqueidentifier NOT NULL,
)

INSERT predmety (id, nazev, zkratka) VALUES ('cf761eca-04bb-4dcd-80ea-0e7d04e3c0d2', 'Programovani', 'PRG')
GO
INSERT predmety (id, nazev, zkratka) VALUES ('3c1470ee-b335-4252-9278-11bb0f7643cc', 'Vyvoj aplikaci', 'VAP')
GO
INSERT predmety (id, nazev, zkratka) VALUES ('28a6588a-a569-4678-abfe-f0bdd804109d', 'Kyberneticka bezpecnost', 'KYB')
GO
INSERT studenti (id, jmeno, prijmeni, datum_narozeni, isic) VALUES ('2379b96a-4f2d-499e-9a90-43e847c57833', 'Jan', 'Novak', NULL, NULL)
GO
INSERT studenti (id, jmeno, prijmeni, datum_narozeni, isic) VALUES ('bb61532a-cd81-4c2a-9c5c-6059b84d8d5b', 'Petra', 'Rychtarova', NULL, NULL)
GO
INSERT studenti (id, jmeno, prijmeni, datum_narozeni, isic) VALUES ('6fba07c8-d87e-4107-b10f-7b256a0d2909', 'Frantisek', 'Lamac', NULL, NULL)
GO
INSERT studenti (id, jmeno, prijmeni, datum_narozeni, isic) VALUES ('12ef1e59-a66a-4368-9613-a69941e29135', 'Marketa', 'Kratka', NULL, NULL)
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('440e2859-26ed-4cbe-80f6-09ece56e499e', 3.0, 5, 'cf761eca-04bb-4dcd-80ea-0e7d04e3c0d2', '', 'bb61532a-cd81-4c2a-9c5c-6059b84d8d5b')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('c6ba5a82-f6bc-4381-b39e-0e5fee09c170', 4.0, 2, '3c1470ee-b335-4252-9278-11bb0f7643cc', '', '12ef1e59-a66a-4368-9613-a69941e29135')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('23acfffd-4074-44a8-a777-10a246a13e34', 1.0, 5, '28a6588a-a569-4678-abfe-f0bdd804109d', '', '6fba07c8-d87e-4107-b10f-7b256a0d2909')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('08221d55-8bd2-43cd-8fe1-19d51dfef231', 2.0, 5, 'cf761eca-04bb-4dcd-80ea-0e7d04e3c0d2', '', '12ef1e59-a66a-4368-9613-a69941e29135')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('2df89b02-fc65-41a7-a898-1a710fbb11a8', 1.5, 5, '28a6588a-a569-4678-abfe-f0bdd804109d', '', '6fba07c8-d87e-4107-b10f-7b256a0d2909')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('8a636e98-2797-4433-9acf-1d7ce64be3e3', 2.0, 5, '3c1470ee-b335-4252-9278-11bb0f7643cc', '', '6fba07c8-d87e-4107-b10f-7b256a0d2909')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('dafe580d-8cfe-4a1e-b503-1e2bd3298fce', 1.0, 10, '3c1470ee-b335-4252-9278-11bb0f7643cc', '', 'bb61532a-cd81-4c2a-9c5c-6059b84d8d5b')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('e1bc27ed-52d1-4c15-83ba-210b94f83420', 3.0, 5, 'cf761eca-04bb-4dcd-80ea-0e7d04e3c0d2', '', '6fba07c8-d87e-4107-b10f-7b256a0d2909')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('a4b53870-0c71-4f46-9407-23c27e977c3f', 2.0, 5, 'cf761eca-04bb-4dcd-80ea-0e7d04e3c0d2', '', '12ef1e59-a66a-4368-9613-a69941e29135')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('3c19740b-9ac6-4124-8730-33ac50584b75', 3.0, 5, '3c1470ee-b335-4252-9278-11bb0f7643cc', '', '12ef1e59-a66a-4368-9613-a69941e29135')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('cf38b2c2-f035-4534-8432-38c4e86d0b0a', 2.0, 4, '3c1470ee-b335-4252-9278-11bb0f7643cc', '', '12ef1e59-a66a-4368-9613-a69941e29135')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('64a7aa5e-e611-4354-98e3-4ec42c5fb759', 3.0, 5, 'cf761eca-04bb-4dcd-80ea-0e7d04e3c0d2', '', '12ef1e59-a66a-4368-9613-a69941e29135')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('8decc347-0c4b-4fff-bf24-4f8d773dcbdd', 3.0, 5, 'cf761eca-04bb-4dcd-80ea-0e7d04e3c0d2', '', 'bb61532a-cd81-4c2a-9c5c-6059b84d8d5b')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('5ebfde78-2357-44b2-b930-58be403e8a59', 4.0, 5, '28a6588a-a569-4678-abfe-f0bdd804109d', '', '12ef1e59-a66a-4368-9613-a69941e29135')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('abadf2f6-c770-41ad-aad6-6ae2c832c40f', 3.0, 2, '3c1470ee-b335-4252-9278-11bb0f7643cc', '', '6fba07c8-d87e-4107-b10f-7b256a0d2909')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('1443e625-bbae-4d3d-ad1e-77acaea0ea56', 2.0, 4, '3c1470ee-b335-4252-9278-11bb0f7643cc', '', '6fba07c8-d87e-4107-b10f-7b256a0d2909')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('dcee55a0-2676-48ee-919b-7f1e6a9bf22c', 3.5, 10, '3c1470ee-b335-4252-9278-11bb0f7643cc', '', '12ef1e59-a66a-4368-9613-a69941e29135')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('966c4a16-e413-4c12-8172-8a696d127372', 1.0, 2, '3c1470ee-b335-4252-9278-11bb0f7643cc', '', 'bb61532a-cd81-4c2a-9c5c-6059b84d8d5b')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('344a50e5-d64b-43ef-aba9-99858cd083b0', 2.0, 5, '28a6588a-a569-4678-abfe-f0bdd804109d', '', 'bb61532a-cd81-4c2a-9c5c-6059b84d8d5b')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('6336c244-507f-4e8d-8e71-9c98744f6346', 3.0, 5, '28a6588a-a569-4678-abfe-f0bdd804109d', '', '12ef1e59-a66a-4368-9613-a69941e29135')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('b1e1f013-9db9-4c3a-b86d-9cbce5b9c81c', 3.0, 5, '28a6588a-a569-4678-abfe-f0bdd804109d', '', 'bb61532a-cd81-4c2a-9c5c-6059b84d8d5b')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('90d8f774-adfa-40cd-85ba-9f79365c369a', 2.0, 5, 'cf761eca-04bb-4dcd-80ea-0e7d04e3c0d2', '', 'bb61532a-cd81-4c2a-9c5c-6059b84d8d5b')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('56b56914-31e6-4e21-9857-a1ed7ce0d3d2', 1.0, 5, 'cf761eca-04bb-4dcd-80ea-0e7d04e3c0d2', '', '6fba07c8-d87e-4107-b10f-7b256a0d2909')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('ff05acae-67ff-4058-ac70-bbe7ab234379', 3.0, 10, '3c1470ee-b335-4252-9278-11bb0f7643cc', '', '6fba07c8-d87e-4107-b10f-7b256a0d2909')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('d7e23e70-9782-4ec4-9c36-bc592a2f331d', 1.0, 10, '3c1470ee-b335-4252-9278-11bb0f7643cc', NULL, 'bb61532a-cd81-4c2a-9c5c-6059b84d8d5b')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('102087fc-2a6f-4637-b8b8-caf243940bf9', 4.0, 5, 'cf761eca-04bb-4dcd-80ea-0e7d04e3c0d2', '', 'bb61532a-cd81-4c2a-9c5c-6059b84d8d5b')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('73407374-0805-4935-be45-dfded6546ee0', 2.0, 4, '3c1470ee-b335-4252-9278-11bb0f7643cc', '', 'bb61532a-cd81-4c2a-9c5c-6059b84d8d5b')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('085abb79-94ea-4f31-aea7-e7071d867517', 2.0, 5, '28a6588a-a569-4678-abfe-f0bdd804109d', '', '12ef1e59-a66a-4368-9613-a69941e29135')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('b0f75d2a-6787-4be4-8097-eef8e8b5ee07', 2.0, 5, 'cf761eca-04bb-4dcd-80ea-0e7d04e3c0d2', '', '6fba07c8-d87e-4107-b10f-7b256a0d2909')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('5c8c36a1-8773-48fc-a338-f036e037716b', 2.0, 5, '28a6588a-a569-4678-abfe-f0bdd804109d', '', 'bb61532a-cd81-4c2a-9c5c-6059b84d8d5b')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('ba00c06f-ed79-490f-85e1-f17c225dfefd', 1.0, 5, 'cf761eca-04bb-4dcd-80ea-0e7d04e3c0d2', '', '6fba07c8-d87e-4107-b10f-7b256a0d2909')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('e0ddfac3-72c4-400d-8e9b-f5843d6bf79f', 2.0, 5, '3c1470ee-b335-4252-9278-11bb0f7643cc', '', 'bb61532a-cd81-4c2a-9c5c-6059b84d8d5b')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('44d5800c-e847-4424-ab3e-faf054a36546', 1.0, 5, '28a6588a-a569-4678-abfe-f0bdd804109d', '', '6fba07c8-d87e-4107-b10f-7b256a0d2909')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('02ffef45-f822-4d4a-9fc2-ff76de2b80eb', 1.0, 5, 'cf761eca-04bb-4dcd-80ea-0e7d04e3c0d2', '', '12ef1e59-a66a-4368-9613-a69941e29135')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('95ecf1ae-7fb4-40bb-b94e-7945a985335d', 2.0, 5, '3c1470ee-b335-4252-9278-11bb0f7643cc', '', '2379b96a-4f2d-499e-9a90-43e847c57833')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('7cba6417-4494-47b1-b582-e9beea3d4e1f', 1.0, 5, '28a6588a-a569-4678-abfe-f0bdd804109d', '', '2379b96a-4f2d-499e-9a90-43e847c57833')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('6cc5e311-0002-45e5-b037-9bde0298ce55', 3.0, 5, '3c1470ee-b335-4252-9278-11bb0f7643cc', '', '2379b96a-4f2d-499e-9a90-43e847c57833')
GO
INSERT znamky (id, hodnota, vaha, idPredmety, poznamka, idStudenti) VALUES ('1294b2b8-d06f-47c2-93d4-cc1e746e5153', 2.0, 5, '28a6588a-a569-4678-abfe-f0bdd804109d', '', '2379b96a-4f2d-499e-9a90-43e847c57833')
GO
ALTER TABLE predmety ADD  CONSTRAINT DF_predmety_id  DEFAULT (newid()) FOR id
GO
ALTER TABLE studenti ADD  CONSTRAINT DF_studenti_id  DEFAULT (newid()) FOR id
GO
ALTER TABLE znamky ADD  CONSTRAINT DF_znamky_id  DEFAULT (newid()) FOR id
GO
ALTER TABLE znamky  WITH CHECK ADD CONSTRAINT FK_znamky_predmety FOREIGN KEY(idPredmety) REFERENCES predmety (id)
GO
ALTER TABLE znamky CHECK CONSTRAINT FK_znamky_predmety
GO
ALTER TABLE znamky  WITH CHECK ADD  CONSTRAINT FK_znamky_studenti FOREIGN KEY(idStudenti) REFERENCES studenti (id)
GO
ALTER TABLE znamky CHECK CONSTRAINT FK_znamky_studenti
GO
ALTER TABLE znamky  WITH CHECK ADD  CONSTRAINT CK_znamky CHECK  ((hodnota=(1) OR hodnota=(1.5) OR hodnota=(2) OR hodnota=(2.5) OR hodnota=(3) OR hodnota=(3.5) OR hodnota=(4) OR hodnota=(4.5) OR hodnota=(5)))
GO
ALTER TABLE znamky CHECK CONSTRAINT CK_znamky
GO

