CREATE PROC CreatePointsTable AS
BEGIN
	CREATE TABLE Points (id INT IDENTITY(1, 1) PRIMARY KEY, pos [dbo].[Point])
END;
GO

CREATE PROC DropPointsTable AS
BEGIN
	DROP TABLE Points;
END;
GO

CREATE PROC CreatePolygonsTable AS
BEGIN
	CREATE TABLE Polygons (id INT IDENTITY(1, 1) PRIMARY KEY, polygon [dbo].[Polygon]);
END;
GO

CREATE PROC DropPolygonsTable AS
BEGIN
	DROP TABLE Polygons;
END;
GO

CREATE PROC FillPolygon(@id INT) AS
BEGIN
	DECLARE @point [dbo].[Point];
	DECLARE curs CURSOR LOCAL STATIC READ_ONLY FORWARD_ONLY
		FOR SELECT pos FROM Points;
	
	OPEN curs;
	FETCH NEXT FROM curs INTO @point;
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		UPDATE Polygons
		SET polygon.NewPoint = @point
		WHERE id = @id;

		FETCH NEXT FROM curs INTO @point;
	END
	
	UPDATE Polygons
	SET polygon.Area = (SELECT polygon.CalculateArea()
						FROM Polygons
						WHERE id = @id)
	WHERE id = @id;

	CLOSE curs;
	DEALLOCATE curs;
END;
