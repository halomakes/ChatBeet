import { FormGroup } from "@angular/forms";

export function touchAllControls(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(key => {
        formGroup.get(key)?.markAsTouched();
    });
};