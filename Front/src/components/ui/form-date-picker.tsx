import * as React from "react";
import { Control, FieldPath, FieldValues, Controller } from "react-hook-form";
import { format, parse, isValid } from "date-fns";
import { ptBR } from "date-fns/locale";

import { Label } from "@/components/ui/label";
import { DatePicker } from "@/components/ui/date-picker";

interface FormDatePickerProps<
  TFieldValues extends FieldValues = FieldValues,
  TName extends FieldPath<TFieldValues> = FieldPath<TFieldValues>
> {
  control: Control<TFieldValues>;
  name: TName;
  label?: string;
  placeholder?: string;
  disabled?: boolean;
  className?: string;
  error?: string;
}

export function FormDatePicker<
  TFieldValues extends FieldValues = FieldValues,
  TName extends FieldPath<TFieldValues> = FieldPath<TFieldValues>
>({
  control,
  name,
  label,
  placeholder = "Selecione uma data",
  disabled = false,
  className,
  error,
}: FormDatePickerProps<TFieldValues, TName>) {
  return (
    <div className={`space-y-2 ${className || ''}`}>
      {label && <Label>{label}</Label>}
      <Controller
        control={control}
        name={name}
        render={({ field, fieldState }) => {
          // Converter string para Date para o DatePicker
          const dateValue = field.value ? stringToDate(field.value) : undefined;

          return (
            <>
              <DatePicker
                value={dateValue}
                onChange={(date) => {
                  // Converter Date para string no formato YYYY-MM-DD para o backend
                  const stringValue = date ? dateToString(date) : "";
                  field.onChange(stringValue);
                }}
                placeholder={placeholder}
                disabled={disabled}
              />
              {(fieldState.error || error) && (
                <p className="text-sm text-red-500">
                  {fieldState.error?.message || error}
                </p>
              )}
            </>
          );
        }}
      />
    </div>
  );
}

// Função para converter string para Date
function stringToDate(dateString: string): Date | undefined {
  if (!dateString || dateString.trim() === '') return undefined;

  // Se a string tem apenas 4 caracteres, assume que é apenas o ano
  if (dateString.length === 4) {
    const year = parseInt(dateString);
    if (!isNaN(year)) {
      return new Date(year, 0, 1); // 1º de janeiro do ano
    }
  }

  // Se já está no formato YYYY-MM-DD
  if (dateString.match(/^\d{4}-\d{2}-\d{2}$/)) {
    const date = new Date(dateString + 'T00:00:00');
    if (isValid(date)) {
      return date;
    }
  }

  // Tentar outros formatos
  try {
    const date = new Date(dateString);
    if (isValid(date)) {
      return date;
    }
  } catch (error) {
    console.warn('Erro ao converter string para data:', dateString, error);
  }

  return undefined;
}

// Função para converter Date para string no formato YYYY-MM-DD
function dateToString(date: Date): string {
  if (!date || !isValid(date)) return "";
  
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  
  return `${year}-${month}-${day}`;
}
