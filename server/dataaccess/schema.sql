drop schema if exists DeadPigeonsDB cascade;
create schema if not exists DeadPigeonsDB;

create table DeadPigeonsDB.Player(
                                     playerId text not null primary key,
                                     name text not null,
                                     phone text not null,
                                     email text not null,
                                     active boolean not null,
                                     createdAt timestamp not null,
                                     updatedAt timestamp not null
);

create table DeadPigeonsDB.Game(
                                   gameId text not null primary key,
                                   weekIdentity timestamp not null,
                                   winningNumbers int[3],
                                   cutoffTime time not null,
                                   createdAt timestamp not null
);


create table DeadPigeonsDB.RepeatingBoard(
                                             repeatingBoardId text not null primary key,
                                             playerId text not null,
                                             isRepeating boolean default false,

                                             foreign key (playerId)
                                                 references DeadPigeonsDB.Player(playerId)
);

create table DeadPigeonsDB.Board(
                                    boardId text not null primary key,
                                    playerId text not null,
                                    gameId text not null,
                                    chosenNumbers int check ( chosenNumbers between 1 and 16),
                                    isWinningBoard boolean not null,
                                    price decimal(10,2) not null check ( price >= 0 ),
                                    repeatingBoardId text null,

                                    foreign key (playerId)
                                        references DeadPigeonsDB.Player(playerId),
                                    foreign key (gameId)
                                        references DeadPigeonsDB.Game(gameId),
                                    foreign key (repeatingBoardId)
                                        references DeadPigeonsDB.RepeatingBoard(repeatingBoardId)
);

create type transaction_status as ENUM ('pending', 'approved', 'declined');

create table DeadPigeonsDB.Transaction(
                                          transactionId text not null primary key,
                                          playerId text not null,
                                          amount int not null,
                                          mobilepayTransactionNumber text not null,
                                          status transaction_status not null default 'pending',
                                          createdAt timestamp not null,

                                          foreign key (playerId)
                                              references DeadPigeonsDB.Player(playerId)
);
