BEGIN TRANSACTION;
		INSERT INTO Halls (RouteHallID, Name) VALUES (1, "Boulder Hall"), (2, "Sports Hall");
		INSERT INTO Sections (RouteSectionID, RouteHallID, Name) VALUES (1, 1, "A"), (2, 1, "B"), (3, 1, "C"), (4, 1, "D"),
																		(5, 2, "A"), (6, 2, "B"), (7, 2, "C"), (8, 2, "D");
		INSERT INTO Difficulties (RouteDifficultyID, Name) VALUES (1, "Green"), (2, "Blue"), (3, "Red"), (4, "Black");
COMMIT;
