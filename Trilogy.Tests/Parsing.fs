﻿module Trilogy.Tests.Parsing

open NUnit.Framework
open Assertions
open Trilogy
open Trilogy.Parser

[<Test>]
let ``select statement``() =
    let expected = SelectStatement {
        Selections = [Unaliased(IdExpr(Named "Name")); Unaliased(ConstExpr Int)]
        Tables = ["T1"; "T2"; "T3"]
        Filter = None
    }
    assertEq expected (parse "select Name, 0 from T1 join T2 on 0 join T3 on 0")

    let expected2 = SelectStatement {
        Selections = [Unaliased(IdExpr(Named "Name")); Unaliased(ConstExpr Int)]
        Tables = ["T1"; "T2"; "T3"]
        Filter = Some(ConstExpr Int)
    }
    assertEq expected2 (parse "select Name, 0 from T1 join T2 on 0 join T3 on 0 where 0")

[<Test>]
let ``select *``() =
    let expected = SelectStatement {
        Selections = [Star None]
        Tables = ["Table"]
        Filter = None
    }
    assertEq expected (parse "select * from Table")

[<Test>]
let ``select Table.*``() =
    let expected = SelectStatement {
        Selections = [Star(Some "Table")]
        Tables = ["Table"]
        Filter = None
    }
    assertEq expected (parse "select Table.* from Table")

[<Test>]
let ``select count``() =
    let expected = SelectStatement {
        Selections = [Unaliased(CountExpr(IdExpr(Named "Thing")))]
        Tables = ["Things"]
        Filter = None
    }
    assertEq expected (parse "select count(Thing) from Things")

[<Test>]
let ``select cast``() =
    let expected = SelectStatement {
        Selections = [Unaliased(CastExpr(IdExpr(Named "Thing"), Int))]
        Tables = ["Things"]
        Filter = None
    }
    assertEq expected (parse "select cast(Thing, int) from Things")

[<Test>]
let ``select as``() =
    let expected = SelectStatement {
        Selections = [Aliased(ConstExpr Int, "X")]
        Tables = ["Y"]
        Filter = None
    }
    assertEq expected (parse "select 0 as X from Y")

[<Test>]
let ``insert``() =
    let expected = InsertStatement {
        Table = "Tbl"
        Columns = ["X"; "Y"; "Z"]
        Values = [ConstExpr Int; ConstExpr Varchar; ConstExpr Int]
    }
    assertEq expected (parse "insert into Tbl (X, Y, Z) values (1, 'a', 53434)")

[<Test>]
let ``update``() =
    let expected = UpdateStatement {
        Table = "Tbl"
        Assignments = ["X", ConstExpr Int; "Y", ConstExpr Varchar]
        Filter = None
    }
    assertEq expected (parse "update Tbl set X = 0, Y = 'a'")

    let expected2 = UpdateStatement {
        Table = "Tbl"
        Assignments = ["X", ConstExpr Int; "Y", ConstExpr Varchar]
        Filter = Some(ConstExpr Int)
    }
    assertEq expected2 (parse "update Tbl set X = 0, Y = 'a' where 0")

[<Test>]
let ``delete``() =
    let expected = InsertStatement {
        Table = "Tbl"
        Columns = ["X"; "Y"]
        Values = [ConstExpr Int; ConstExpr Varchar]
    }
    assertEq expected (parse "insert into Tbl (X, Y) values (0, 'a')")

[<Test>]
let ``create``() =
    let expected = CreateStatement {
        Name = "Thingy"
        Columns = ["X", Int; "Y", Varchar]
    }
    assertEq expected (parse "create table Thingy ( X int, Y varchar )")

[<Test>]
let ``create, select``() =
    let expected =
        [
            CreateStatement {
                Name = "Tbl"
                Columns = ["X", Int; "Y", Varchar]
            }
            SelectStatement {
                Selections = [Unaliased(IdExpr(Named "Y"))]
                Tables = ["Tbl"]
                Filter = Some(ConstExpr Int)
            }
        ]
    assertEq expected (parseAll "create table Tbl (X int, Y varchar)
                                 select Y from Tbl where 5")

[<Test>]
let ``case insensitivity``() =
    parseAll "SELECT X From Table" |> ignore
    ()
