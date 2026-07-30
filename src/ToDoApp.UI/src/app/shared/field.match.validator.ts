import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function fieldsMatchValidator(
  first: string,
  second: string
): ValidatorFn
{
  return (control: AbstractControl): ValidationErrors | null =>
  {
    const firstValue = control.get(first)?.value;
    const secondValue = control.get(second)?.value;
    return firstValue === secondValue ? null : { fieldsMismatch: true };
  };
}
