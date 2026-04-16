PROJECT := ./src/App/App.csproj
OUT := ./src/App/bin/Release/publish
RIDS := linux-x64 win-x64 win-x86
VERSION_FILE := ./Directory.Build.props
VERSION := $(shell sed -n 's:.*<Version>\([^<]*\)</Version>.*:\1:p' $(VERSION_FILE) | head -n 1)

.PHONY: publish

publish:
	@if [ ! -f "$(VERSION_FILE)" ]; then \
		echo "ERROR: no existe $(VERSION_FILE)"; \
		exit 1; \
	fi
	@if [ -z "$(VERSION)" ]; then \
		echo "ERROR: no se pudo obtener <Version> desde $(VERSION_FILE)"; \
		exit 1; \
	fi
	@echo "Publicando versión $(VERSION)"
	for rid in $(RIDS); do \
		outdir="$(OUT)/cc-cli-v$(VERSION)-$$rid"; \
		echo ">> $$rid -> $$outdir"; \
		dotnet publish $(PROJECT) -c Release -r $$rid \
			/p:PublishSingleFile=true \
			/p:Version=$(VERSION) \
			-o "$$outdir"; \
	done
	@echo "Listo."
