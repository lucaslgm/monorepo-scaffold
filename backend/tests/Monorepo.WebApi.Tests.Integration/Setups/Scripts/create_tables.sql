IF OBJECT_ID('CLIENTES', 'U') IS NULL
CREATE TABLE CLIENTES
(
    CLIENTE CHAR(15) NOT NULL PRIMARY KEY,
    NOME    VARCHAR(255),
    CPF     CHAR(11)
);

IF OBJECT_ID('dbo.Func', 'FN') IS NULL
BEGIN
EXEC('
        CREATE FUNCTION dbo.Func
        (
            @Contrato VARCHAR(20),
            @Tipo CHAR(1)
        )
        RETURNS VARCHAR(20)
        AS
        BEGIN
            RETURN ''Novo''
        END
    ');
END;
