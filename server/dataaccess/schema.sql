drop schema if exists DeadPigeonsDB cascade;
create schema if not exists DeadPigeonsDB;

create table DeadPigeonsDB.Player(
                                     playerId uuid not null primary key,
                                     name text not null,
                                     phone text not null,
                                     email text not null,
                                     active boolean not null,
                                     createdAt timestamp not null,
                                     updatedAt timestamp not null
);

create table DeadPigeonsDB.Game(
                                   gameId uuid not null primary key,
                                   weekIdentity timestamp not null,
                                   winningNumbers int[3],
                                   cutoffTime time not null,
                                   createdAt timestamp not null
);


create table DeadPigeonsDB.RepeatingBoard(
                                             repeatingBoardId uuid not null primary key,
                                             playerId uuid not null,
                                             isRepeating boolean not null default false,

                                             foreign key (playerId)
                                                 references DeadPigeonsDB.Player(playerId)
);

create table DeadPigeonsDB.Board(
                                    boardId uuid not null primary key,
                                    playerId uuid not null,
                                    gameId uuid not null,
                                    chosenNumbers int check ( chosenNumbers between 1 and 16),
                                    isWinningBoard boolean not null,
                                    price decimal(10,2) not null check ( price >= 0 ),
                                    repeatingBoardId uuid null,

                                    foreign key (playerId)
                                        references DeadPigeonsDB.Player(playerId),
                                    foreign key (gameId)
                                        references DeadPigeonsDB.Game(gameId),
                                    foreign key (repeatingBoardId)
                                        references DeadPigeonsDB.RepeatingBoard(repeatingBoardId)
);



create table DeadPigeonsDB.Transaction(
                                          transactionId uuid not null primary key,
                                          playerId uuid not null,
                                          amount int not null,
                                          mobilepayTransactionNumber text not null,
                                          status text not null 
                                              default 'pending'
                                              check (status in ('pending','approved','declined')),
                                          createdAt timestamp not null default now(),

                                          foreign key (playerId)
                                              references DeadPigeonsDB.Player(playerId)
);
