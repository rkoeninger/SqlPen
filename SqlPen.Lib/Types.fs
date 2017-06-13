﻿namespace SqlPen

type SqlType =
    | Bit
    | Int
    | Varchar
    | NVarchar

type SqlTypeBounds =
    | Any
    | Limits of Set<SqlType> // If set is empty, can't be anything

type Op =
    | Eq
    | Gt
    | Lt
    | And
    | Or

type Columns = (string * SqlType) list

type Id =
    | Qualified of string * Id
    | Named of string
    | Param of string
    | Unnamed
    | Star

type Projection = (string option * SqlType) list

type SqlExpr =
    | ConstExpr of SqlType
    | IdExpr of Id
    | AliasExpr of SqlExpr * string
    | CastExpr of SqlExpr * SqlType
    | CountExpr of SqlExpr
    | BinaryExpr of Op * SqlExpr * SqlExpr

type Sources = (string * Columns) list

type SelectStmt = { Selections: SqlExpr list; Sources: Sources }

type WhereClause = { Condition: SqlExpr; Sources: Sources }

type Parameters = Map<string, SqlTypeBounds>

type Mode = Union | Intersection

type Select = {
    Expressions: SqlExpr list
    Tables: string list
    Filter: SqlExpr
}

type Insert = {
    Table: string
    Columns: string list
    Values: SqlExpr list
}