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
		outdir="$(OUT)/cc-cli-v$(VERSION)-$$rid"; \
		echo ">> $$rid -> $$outdir"; \
		dotnet publish $(PROJECT) -c Release -r $$rid \
			/p:PublishSingleFile=true \
			/p:Version=$(VERSION) \
			-o "$$outdir"; \
	done
	@echo "Listo."

version:
	@npx standard-version --skip.commit=true --skip.tag=true
	@version=$$(sed -n 's:^## \[\([0-9][^]]*\)\].*:\1:p' CHANGELOG.md | head -n 1); \
	if [ -z "$$version" ]; then \
		echo "ERROR: no se pudo obtener la nueva versión desde CHANGELOG.md"; \
		exit 1; \
	fi; \
	sed -i 's:<Version>[^<]*</Version>:<Version>'"$$version"'</Version>:' Directory.Build.props; \
	echo "Version sincronizada en Directory.Build.props: $$version"
