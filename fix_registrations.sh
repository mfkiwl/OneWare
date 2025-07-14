#!/bin/bash

echo "Fixing Autofac registration syntax..."

# Fix registrations that are missing .AsSelf()
find src -name "*Module.cs" -exec sed -i 's/builder\.RegisterType<\([^>]*\)>();/builder.RegisterType<\1>().AsSelf();/g' {} \;

# Fix registrations that are missing .SingleInstance() for singletons
find src -name "*Module.cs" -exec sed -i 's/builder\.RegisterType<\([^>]*\)>()\.AsSelf()\.SingleInstance();/builder.RegisterType<\1>().AsSelf().SingleInstance();/g' {} \;

# Fix common patterns for service registrations
find src -name "*Module.cs" -exec sed -i 's/builder\.RegisterType<\([^>]*\)>\(\.[^;]*\)\.SingleInstance();/builder.RegisterType<\1>()\2.SingleInstance();/g' {} \;

echo "Registration syntax fixed!"
