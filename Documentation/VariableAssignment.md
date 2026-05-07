The "Variable Assignments" feature is now fully implemented and  supports:
-	Standard assignments (x = 5, var y = 10, order = new Order())
-	Member/property assignments (obj.Property = value, this._field = value)
-	Indexer assignments (array[0] = 5, dict["key"] = 100)
-	Compound assignments (total += 10, flags |= 1, text ??= "default")
-	Auto-property initializers (public int Id { get; set; } = 1)
-	Tuple/deconstruction assignments ((int x, int y) = GetTuple())
-	Multiple assignments (int x = 1, y = 2)