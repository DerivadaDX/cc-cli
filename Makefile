PROJECT := ./src/App/App.csproj
OUT := ./src/App/bin/Release/publish
RIDS := linux-x64 win-x64 win-x86

.PHONY: publish version

publish:
	@if [ -z "$(VERSION)" ]; then \
		echo "ERROR: falta VERSION (ej: make publish VERSION=1.2.3)"; \
		exit 1; \
	fi
	@echo "Publicando versión $(VERSION)"
	for rid in $(RIDS); do \
		echo ">> $$rid"; \
		dotnet publish $(PROJECT) -c Release -r $$rid \
			/p:PublishSingleFile=true \
			/p:Version=$(VERSION) \
			-o $(OUT)/$$rid; \
	done
	@echo "Listo."

version:
	npx standard-version --skip.commit=true --skip.tag=true
