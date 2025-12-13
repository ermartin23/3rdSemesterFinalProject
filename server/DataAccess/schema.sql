drop schema if exists DeadPigeonsDB cascade;
create schema if not exists DeadPigeonsDB;

create table DeadPigeonsDB.Player(
                                     playerId uuid not null primary key,
                                     name text not null,
                                     phone text not null,
                                     email text not null,
                                     password text not null,
                                     active boolean not null,
                                     createdAt timestamptz not null,
                                     updatedAt timestamptz not null,

                                     isDeleted  boolean     not null default false,
                                     deletedAt  timestamptz null
);

create table DeadPigeonsDB.Admin(
                                    adminId    uuid        not null primary key,
                                    name       text        not null,
                                    phone      text        not null,
                                    email      text        not null,
                                    password   text        not null,
                                    createdAt  timestamptz not null,
                                    updatedAt  timestamptz not null,

                                    isDeleted  boolean     not null default false,
                                    deletedAt  timestamptz null
);

create table DeadPigeonsDB.Game(
                                   gameId uuid not null primary key,
                                   weekIdentity timestamptz not null,
                                   winningNumbers int[3],
                                   cutoffTime time not null,
                                   createdAt timestamptz not null,

                                   isDeleted     boolean     not null default false,
                                   deletedAt     timestamptz null
);


create table DeadPigeonsDB.RepeatingBoard(
                                             repeatingBoardId uuid not null primary key,
                                             playerId uuid not null,
                                             isRepeating boolean not null default false,

                                             isDeleted        boolean     not null default false,
                                             deletedAt        timestamptz null,

                                             foreign key (playerId)
                                                 references DeadPigeonsDB.Player(playerId)
);

create table DeadPigeonsDB.Board(
                                    boardId uuid not null primary key,
                                    playerId uuid not null,
                                    gameId uuid not null,
                                    chosenNumbers int[] not null,
                                    isWinningBoard boolean not null,
                                    price decimal(10,2) not null check ( price >= 0 ),
                                    repeatingBoardId uuid null,

                                    isDeleted        boolean     not null default false,
                                    deletedAt        timestamptz null,

                                    foreign key (playerId)
                                        references DeadPigeonsDB.Player(playerId),
                                    foreign key (gameId)
                                        references DeadPigeonsDB.Game(gameId),
                                    foreign key (repeatingBoardId)
                                        references DeadPigeonsDB.RepeatingBoard(repeatingBoardId)
);

alter table DeadPigeonsDB.Board
    add constraint chosen_numbers_count check (
        array_length(chosenNumbers, 1) between 5 and 8
        );
alter table DeadPigeonsDB.Board
    add constraint chosen_numbers_values check (
        chosenNumbers <@ ARRAY[1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16]
    );



create table DeadPigeonsDB.Transaction(
                                          transactionId uuid not null primary key,
                                          playerId uuid not null,
                                          amount int not null,
                                          mobilepayTransactionNumber text not null,
                                          status text not null
                                                                       default 'pending'
                                              check (status in ('pending','approved','declined')),
                                          createdAt timestamptz not null default now(),

                                          isDeleted                  boolean     not null default false,
                                          deletedAt                  timestamptz null,

                                          foreign key (playerId)
                                              references DeadPigeonsDB.Player(playerId)
);
