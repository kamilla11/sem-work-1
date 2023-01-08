-- СОЗДАНИЕ БАЗЫ ДАННЫХ

CREATE DATABASE Ermitage;

-- СОЗДАНИЕ ТАБЛИЦ

CREATE TABLE Account
(
    id       serial primary key,
    email    varchar(100) not null,
    password varchar(50)  not null,
    name     varchar(50)  not null,
    surname  varchar(50)  not null,
    gender   varchar(30)  not null
);

CREATE TABLE Session
(
    id             uuid primary key,
    accountId      int  not null,
    createDateTime date not null,
    constraint fk_ses_acc foreign key (accountId) --restrict
        references Account (id)
        on delete cascade on update cascade
);

CREATE TABLE Event
(
    id          serial primary key,
    name        varchar(250) unique not null,
    speaker     varchar(250)        not null,
    date        varchar(50)         not null,
    place       varchar(250)        not null,
    price       decimal(13, 2) check (price > 0),
    description text
);

CREATE TABLE Ticket
(
    id    serial primary key,
    name  varchar(50) unique not null,
    price decimal(13, 2) check (price > 0)
);

CREATE TABLE Exposition
(
    id          serial primary key,
    name        varchar(250) unique not null,
    place       varchar(250)        not null,
    description text,
    imgPath     varchar(100)        not null
);

CREATE TABLE Painting
(
    id         serial primary key,
    name       varchar(250) unique not null,
    artist     varchar(50)         not null,
    exposition int                 not null,
    year       varchar(20)         not null,
    imgPath    varchar(100)        not null,
    constraint fk_paint_artist foreign key (exposition) --restrict
        references Exposition (id)
        on delete cascade on update cascade
);


CREATE TABLE Exhibition
(
    id          serial primary key,
    name        varchar(250) unique not null,
    place       varchar(250)        not null,
    startDate   date                not null,
    endDate     date                not null,
    description text,
    imgPath     varchar(100)        not null
);



CREATE TABLE Comment
(
    id      serial primary key,
    eventId int  not null,
    userId  int  not null,
    email   varchar(100),
    text    text not null,
    constraint fk_com_exh foreign key (eventId) --restrict
        references Event (id)
        on delete cascade on update cascade,
    constraint fk_com_acc foreign key (userId)  --restrict
        references Account (id)
        on delete cascade on update cascade
);

-- ЗАГРУЗКА ДАННЫХ

INSERT INTO Event(id, name, speaker, date, place, price, description)
    OVERRIDING SYSTEM VALUE
VALUES (101, '«Очень женские судьбы». Мари Бракмон и Ева Гонсалес',
        'А. А. Петровская, искусствовед, сотрудник Научно-Просветительного отдела.', '14 декабря 2022, 18:30',
        'Главный штаб. Камерный зал «Диалог искусств».', 650,
        'О судьбах малоизвестных французских художниц, связанных с движением импрессионизма.'),
       (102, '«Горит дом, где семья Царя Русского». Пожар Зимнего дворца 17 декабря 1837 года',
        'Е. С. Измаилкина, сотрудник Научно-Просветительного отдела.', '
16 декабря 2022, 18:30', 'Камерный зал «Диалог искусств»', 500,
        'Рассказ о пожаре Зимнего дворца 1837 года и реконструкции парадных залов и жилых покоев императорской фамилии по указанию Николая I.'),
       (103, 'Путь длинною в век: от синематографа к медиа-арту',
        'Е. А. Кривенцова, искусствовед, сотрудник Научно-Просветительного отдела.', '17 декабря 2022, 15:00',
        'Главный штаб. Камерный зал «Диалог искусств»', 500,
        'О взаимодействии современного художественного процесса и кинематографа, о применении новых технологий в искусстве и работе художников с движущимися изображениями.'),
       (104, '«Из истории русской дипломатии»: А. С. Грибоедов, А. С. Пушкин, Ф. И. Тютчев.',
        'А. А. Тараскина, научный сотрудник Отдела истории Главного штаба.', '18 декабря 2022, 15:00',
        'Главный штаб. Розовый зал № 332.', 650, 'Рассказ о дипломатической службе выдающихся русских поэтов.');

INSERT INTO Ticket(id, name, price)
    OVERRIDING SYSTEM VALUE
VALUES (201, 'Главный музейный комплекс', 500),
       (202, 'Главный штаб ', 500),
       (203, 'Зимний дворец Петра I', 600),
       (204, 'Дворец Меншикова ', 600),
       (205, 'Эрмитаж – Старая Деревня', 650),
       (206, 'Музей Императорского фарфорового завода', 300);


INSERT INTO Exposition(id, name, place, description, imgPath)
    OVERRIDING SYSTEM VALUE
VALUES (301, 'Галерея портретов Романовых', 'Главный музейный комплекс, Зимний дворец II этаж, залы 151, 153',
        'Романовская галерея с 1840 по 1918 гг. размещалась в галерее Малого Эрмитажа. В современном музее она была воссоздана в одной из галерей Зимнего дворца на основе богатого иконографического наследия Дома Романовых из собрания Эрмитажа. Здесь представлены портреты всех царствующих особ династии Романовых и портреты членов их семей.',
        '/assets/images/expositions/img1.jpg'),
       (302, 'Искусство Франции XV–XVIII вв.',
        'Главный музейный комплекс, Зимний дворец, II этаж, залы 272-281, 283-297',
        'Экспозиция, построенная на основе одной из самых обширных и значительных коллекций Эрмитажа, включает в себя работы крупнейших мастеров живописи и представляет разные художественные направления в искусстве Франции XV–XVIII вв. (залы 272-297).',
        '/assets/images/expositions/img2.jpg'),
       (303, 'Искусство Италии эпохи Возрождения XIII–XVI вв.',
        'Искусство эпохи Возрождения от его зарождения в XIII веке и до XVI века представлено в Парадной анфиладе второго этажа Старого Эрмитажа. В экспозиции - работы мастеров разных итальянских школ, ведущее место среди которых занимает флорентийская.',
        'Главный музейный комплекс, Большой Эрмитаж, II этаж, залы 207-216', '/assets/images/expositions/img3.jpg'),
       (304, 'Живопись Фландрии', 'Главный музейный комплекс, Новый Эрмитаж, II этаж, залы 245-247',
        'Фламандская живопись представлена в четырех залах второго этажа Нового Эрмитажа (залы 245-247). Одно из крупнейших собраний в мире включает произведения знаменитых живописцев XVII в. – Питера Пауля Рубенса, Антониса Ван Дейка, Якоба Йорданса, Франса Снейдерса.',
        '/assets/images/expositions/img4.jpg');

INSERT INTO Painting(id, name, artist, exposition, year, imgPath)
    OVERRIDING SYSTEM VALUE
VALUES (401, '«Союз Земли и Воды»', 'Питер Пауль Рубенс', 304, '1618', '/assets/images/paintings/img5.jpg'),
       (402, '«Персей и Андромеда»', 'Питер Пауль Рубенс', 304, '1622', '/assets/images/paintings/img6.jpg'),
       (403, '«Лавки»', 'Франс Снейдерс', 304, '1620', '/assets/images/paintings/img7.jpg'),
       (404, ' «Благовещение»', ' Симоне Мартини и Липпо Мемми', 303, '1333', '/assets/images/paintings/img8.jpg'),
       (405, '«Мадонна Бенуа»', ' Леонардо да Винчи', 303, '1478', '/assets/images/paintings/img9.jpg'),
       (406, '«Мадонна Литта»', ' Леонардо да Винчи', 303, '1490-1491', '/assets/images/paintings/img10.jpg'),
       (407, 'Аллегорический портрет Анны Австрийской ', 'Симона Вуэ', 302, '1643—1645',
        '/assets/images/paintings/img11.jpg'),
       (408, 'Пейзаж с Полифемом', 'Никола Пуссена', 302, '1649', '/assets/images/paintings/img12.jpg'),
       (409, 'Портрет Иоанна VI (Иоанна Антоновича)', 'Неизвестный художник', 301, '1742',
        '/assets/images/paintings/img1.jpg'),
       (410, 'Портрет великого князя Петра Федоровича и великой княгини Екатерины Алексеевны', 'Гроот Г. К.', 301,
        '1745', '/assets/images/paintings/img2.jpg'),
       (411,
        'Портрет Шарлотты Кристины Софии, рожденной принцессы Брауншвейг-Вольфенбюттельской, жены царевича Алексея Петровича',
        'Неизвестный художник(Таннауер И. Г. (?))', 301, '1710', '/assets/images/paintings/img3.jpg'),
       (412, 'Портрет самодержицы Софьи Алексеевны', 'Неизвестный художник', 301, 'Конец XVII в. (?)',
        '/assets/images/paintings/img4.jpg');

INSERT INTO Exhibition(id, name, place, startDate, endDate, description, imgPath)
    OVERRIDING SYSTEM VALUE
VALUES (501, '«Крокус». Возвращение. К 125-летию Николая Суетина', 'Главный штаб, зал-трансформер', '2022-12-03',
        '2023-02-26',
        '3 декабря 2022 года в Главном штабе начинает работу выставка, посвящённая одному из ведущих художников русского авангарда – Николаю Суетину. Мастер мощного и масштабного дарования, он оставил заметный след в живописи, дизайне и архитектуре, но при упоминании его имени в первую очередь представляется фарфор.',
        '/assets/images/exhibitions/img1.jpg'),
       (502, 'Вещь с секретом. Ювелирное искусство XVI – XXI веков', 'Синяя спальня Зимнего дворца (Зал №307)',
        '2022-10-22', '2023-09-11',
        '22 октября 2022 года в Синей спальне Зимнего дворца открывается выставка «Вещь с секретом. Ювелирное искусство XVI–XXI веков». Экспозиция впервые объединяет ювелирные изделия из коллекции Государственного Эрмитажа, содержащие различные секреты: тайники, сюрпризы; скрытые смыслы.',
        '/assets/images/exhibitions/img2.jpg'),
       (503, 'Для любителей отечественной учёности и просвещения. Эрмитажная и Публичная библиотеки. 1762–1862 годы',
        'Двенадцатиколонный зал', '2022-12-07', '2023-03-12',
        'В Дни Эрмитажа, 7 декабря 2022 года, начинает работу выставка «Для любителей отечественной учёности и просвещения. Эрмитажная и Публичная библиотеки. 1762–1862 годы», раскрывающая страницы истории двух уникальных хранилищ бесценных рукописных и печатных книг и документов.',
        '/assets/images/exhibitions/img3.jpg'),
       (504, 'Египтомания. К 200-летию дешифровки египетских иероглифов Ж.-Ф. Шампольоном',
        'Аванзал и Николаевский зал', '2022-10-28', '2023-04-09',
        'С 28 октября 2022 года в Николаевском зале и Аванзале Зимнего дворца открыта одна из главных выставок сезона – «Египтомания. К 200-летию дешифровки египетских иероглифов Жаном-Франсуа Шампольоном.',
        '/assets/images/exhibitions/img4.jpg');

-- ОТОБРАЖЕНИЕ ДАННЫХ

SELECT *
FROM Event;
SELECT *
FROM Ticket;
SELECT *
FROM exposition;
SELECT *
FROM Painting;
SELECT *
FROM Exhibition;