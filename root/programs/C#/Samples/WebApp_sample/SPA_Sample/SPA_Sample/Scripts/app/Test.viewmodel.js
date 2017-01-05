function TestViewModel() {
	// Shipper テーブルの各項目
	this.ShipperId = ko.observable("Shipper1");
	this.CompanyName = ko.observable("Japan");
	this.Phone = ko.observable("052-569-2597");

	this.SomeFunction = function () {
		var currentVal = this.ShipperId();
		this.ShipperId(currentVal.toUpperCase());
	};
}

ko.applyBindings(new TestViewModel());
