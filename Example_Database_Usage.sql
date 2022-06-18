EXEC CreatePointsTable;
EXEC CreatePolygonsTable;
GO

INSERT INTO Points(Pos) VALUES ('1; 2'), ('5;9'), ('-3; 0'), ('1;2');
SELECT pos.ToString() FROM Points;
SELECT pos.DistanceTo('1;10') FROM Points;

INSERT INTO Polygons VALUES ('');
EXEC FillPolygon 1;

SELECT polygon.ToString() FROM Polygons;

SELECT polygon.GetPointCount() FROM Polygons;
SELECT id, polygon.CalculateArea() AS area FROM Polygons;
SELECT polygon.Area FROM Polygons;
SELECT polygon.IsPointInside('1;3') FROM Polygons;
SELECT polygon.IsCoordInside(1, 3) FROM Polygons;

EXEC DropPointsTable;
EXEC DropPolygonsTable;
GO

--DROP PROC CreatePointsTable;
--DROP PROC DropPointsTable;
--DROP PROC CreatePolygonsTable;
--DROP PROC DropPolygonsTable;
--DROP PROC FillPolygon;
--GO
