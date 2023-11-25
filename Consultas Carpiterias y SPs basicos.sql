USE carpinteria_db;
GO;

USE carpinteria_db;

select * from T_PRODUCTOS;
select * from T_PRESUPUESTOS;
select * from T_DETALLES_PRESUPUESTO;

INSERT INTO T_PRODUCTOS VALUES ('AGARRADERA',2000,'S');
INSERT INTO T_PRODUCTOS VALUES ('ASA',500,'S');
INSERT INTO T_PRODUCTOS VALUES ('BISAGRA',8000,'S');

-- PROCEDIMIENTOS ALMACENADOS BÁSICOS DE CARPINTERIA_DB

-- SP PROXIMO ID.
ALTER PROCEDURE [dbo].[SP_PROXIMO_ID]
@next int OUTPUT
AS
BEGIN
	SET @next = (SELECT MAX(presupuesto_nro)+1  FROM T_PRESUPUESTOS);
END
GO;

-- SP CONSULTAR PRODUCTOS / CARGAR COMBO
ALTER PROCEDURE [dbo].[SP_CONSULTAR_PRODUCTOS]
AS
BEGIN
	SELECT * from T_PRODUCTOS;
END;
GO;

-- SP INSERTAR MAESTRO
ALTER PROCEDURE [dbo].[SP_INSERTAR_MAESTRO] 
	--no lleva @id porque es identity
	@cliente varchar(255), 
	@dto numeric(5,2),
	@total numeric(8,2),
	@presupuesto_nro int OUTPUT
AS
BEGIN
	INSERT INTO T_PRESUPUESTOS(fecha, cliente, descuento, total)
    VALUES (GETDATE(), @cliente, @dto, @total);
    --Asignamos el valor del último ID autogenerado (obtenido --  
    --mediante la función SCOPE_IDENTITY() de SQLServer)	
    SET @presupuesto_nro = SCOPE_IDENTITY();
END;
GO;


-- SP INSERTAR DETALLE
ALTER PROCEDURE [dbo].[SP_INSERTAR_DETALLE] 
	@presupuesto_nro int, -- habiendolo traigo por SCOPE_identity() lo usamos para asignar detalle a presu
	@detalle int, 
	@id_producto int, 
	@cantidad int
AS
BEGIN
	INSERT INTO T_DETALLES_PRESUPUESTO(presupuesto_nro,detalle_nro, id_producto, cantidad)
    VALUES (@presupuesto_nro, @detalle, @id_producto, @cantidad);
END;


