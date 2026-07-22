-- =============================================================================
-- Migración: Facturación Electrónica AFIP
-- Ejecutar sobre la base de datos FiyiPOS (SQL Server)
-- =============================================================================

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'CompanySettings')
BEGIN
    CREATE TABLE CompanySettings (
        Id                      INT             IDENTITY(1,1) PRIMARY KEY,
        StoreId                 BIGINT          NOT NULL UNIQUE REFERENCES Store(StoreId),
        CompanyName             NVARCHAR(200)   NOT NULL DEFAULT '',
        LegalName               NVARCHAR(200)   NOT NULL DEFAULT '',
        TaxId                   VARCHAR(11)     NOT NULL DEFAULT '',
        Address                 NVARCHAR(300)   NOT NULL DEFAULT '',
        Phone                   NVARCHAR(50)    NOT NULL DEFAULT '',
        TaxCondition            VARCHAR(50)     NOT NULL DEFAULT 'Monotributista',
        PointOfSale             INT             NOT NULL DEFAULT 1,
        Environment             VARCHAR(20)     NOT NULL DEFAULT 'Homologacion',
        InvoicingEnabled        BIT             NOT NULL DEFAULT 0,
        CertificatePfx          VARBINARY(MAX)  NULL,
        CertificatePassword     NVARCHAR(500)   NULL,
        CertificateSubject      NVARCHAR(500)   NULL,
        CertificateIssuer       NVARCHAR(500)   NULL,
        ExpirationDate          DATETIME        NULL,
        CreatedAt               DATETIME        NOT NULL DEFAULT GETDATE(),
        ModifiedAt              DATETIME        NOT NULL DEFAULT GETDATE()
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'AfipAccessTicket')
BEGIN
    CREATE TABLE AfipAccessTicket (
        Id                      INT             IDENTITY(1,1) PRIMARY KEY,
        CompanySettingsId       INT             NOT NULL REFERENCES CompanySettings(Id) ON DELETE CASCADE,
        ServiceName             VARCHAR(50)     NOT NULL DEFAULT 'wsfe',
        Token                   NVARCHAR(MAX)   NOT NULL,
        Sign                    NVARCHAR(MAX)   NOT NULL,
        ExpirationTime          DATETIME        NOT NULL,
        CreatedAt               DATETIME        NOT NULL DEFAULT GETDATE()
    );

    CREATE UNIQUE INDEX UX_AfipAccessTicket_Company_Service
        ON AfipAccessTicket(CompanySettingsId, ServiceName);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Invoice')
BEGIN
    CREATE TABLE Invoice (
        InvoiceId               BIGINT          IDENTITY(1,1) PRIMARY KEY,
        SaleId                  BIGINT          NOT NULL REFERENCES Sale(SaleId),
        StoreId                 BIGINT          NOT NULL REFERENCES Store(StoreId),
        ClientId                BIGINT          NULL REFERENCES Client(ClientId),
        SystemUserId            BIGINT          NOT NULL REFERENCES SystemUser(SystemUserId),
        InvoiceNumber           VARCHAR(50)     NOT NULL,
        DocumentType            INT             NOT NULL,
        PointOfSale             INT             NOT NULL,
        DocumentNumber          BIGINT          NOT NULL,
        CAE                     VARCHAR(14)     NULL,
        CAEExpirationDate       DATETIME        NULL,
        InvoiceDate             DATETIME        NOT NULL DEFAULT GETDATE(),
        Subtotal                NUMERIC(18,2)   NOT NULL,
        TaxAmount               NUMERIC(18,2)   NOT NULL,
        Total                   NUMERIC(18,2)   NOT NULL,
        Status                  VARCHAR(20)     NOT NULL DEFAULT 'active',
        AfipResult              VARCHAR(1)      NULL,
        AfipErrors              VARCHAR(MAX)    NULL,
        Environment             VARCHAR(20)     NOT NULL DEFAULT 'Homologacion',
        XmlRequest              VARCHAR(MAX)    NULL,
        XmlResponse             VARCHAR(MAX)    NULL,
        CreatedAt               DATETIME        NOT NULL DEFAULT GETDATE(),
        CONSTRAINT UX_Invoice_Fiscal UNIQUE (PointOfSale, DocumentType, DocumentNumber, Environment)
    );

    CREATE INDEX IX_Invoice_SaleId ON Invoice(SaleId);
END
ELSE
BEGIN
    -- La tabla Invoice ya existía (versión CFDI). Se actualizan las columnas para AFIP.

    -- Hacer ClientId nullable si no lo es
    IF EXISTS (
        SELECT 1 FROM sys.columns
        WHERE object_id = OBJECT_ID('Invoice') AND name = 'ClientId' AND is_nullable = 0
    )
    BEGIN
        DECLARE @fk_inv NVARCHAR(200);
        SELECT @fk_inv = fk.name
        FROM sys.foreign_keys fk
        JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
        JOIN sys.columns c ON fkc.parent_object_id = c.object_id AND fkc.parent_column_id = c.column_id
        WHERE fk.parent_object_id = OBJECT_ID('Invoice') AND c.name = 'ClientId';
        IF @fk_inv IS NOT NULL EXEC('ALTER TABLE Invoice DROP CONSTRAINT ' + @fk_inv);
        ALTER TABLE Invoice ALTER COLUMN ClientId BIGINT NULL;
        IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('Invoice') AND name = 'FK_Invoice_Client')
            ALTER TABLE Invoice ADD CONSTRAINT FK_Invoice_Client FOREIGN KEY (ClientId) REFERENCES Client(ClientId);
    END

    -- Agregar columnas AFIP
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'StoreId')
        ALTER TABLE Invoice ADD StoreId BIGINT NOT NULL DEFAULT 0;

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'DocumentType')
        ALTER TABLE Invoice ADD DocumentType INT NOT NULL DEFAULT 11;

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'PointOfSale')
        ALTER TABLE Invoice ADD PointOfSale INT NOT NULL DEFAULT 1;

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'DocumentNumber')
        ALTER TABLE Invoice ADD DocumentNumber BIGINT NOT NULL DEFAULT 0;

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'CAE')
        ALTER TABLE Invoice ADD CAE VARCHAR(14) NULL;

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'CAEExpirationDate')
        ALTER TABLE Invoice ADD CAEExpirationDate DATETIME NULL;

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'AfipResult')
        ALTER TABLE Invoice ADD AfipResult VARCHAR(1) NULL;

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'AfipErrors')
        ALTER TABLE Invoice ADD AfipErrors VARCHAR(MAX) NULL;

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'Environment')
        ALTER TABLE Invoice ADD Environment VARCHAR(20) NOT NULL DEFAULT 'Homologacion';

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'XmlRequest')
        ALTER TABLE Invoice ADD XmlRequest VARCHAR(MAX) NULL;

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'XmlResponse')
        ALTER TABLE Invoice ADD XmlResponse VARCHAR(MAX) NULL;

    -- Eliminar columnas CFDI obsoletas
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'UUID')
        ALTER TABLE Invoice DROP COLUMN UUID;

    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'XmlContent')
        ALTER TABLE Invoice DROP COLUMN XmlContent;

    -- Unique index AFIP
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('Invoice') AND name = 'UX_Invoice_Fiscal')
        CREATE UNIQUE INDEX UX_Invoice_Fiscal ON Invoice (PointOfSale, DocumentType, DocumentNumber, Environment);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'AfipLog')
BEGIN
    CREATE TABLE AfipLog (
        Id              BIGINT          IDENTITY(1,1) PRIMARY KEY,
        StoreId         BIGINT          NOT NULL,
        Operation       VARCHAR(100)    NOT NULL,
        Request         VARCHAR(MAX)    NULL,
        Response        VARCHAR(MAX)    NULL,
        IsError         BIT             NOT NULL DEFAULT 0,
        ErrorMessage    VARCHAR(MAX)    NULL,
        CreatedAt       DATETIME        NOT NULL DEFAULT GETDATE()
    );
END
GO
