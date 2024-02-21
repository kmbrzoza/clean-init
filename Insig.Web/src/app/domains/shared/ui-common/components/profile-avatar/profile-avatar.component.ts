
import { Component, Input, OnInit } from '@angular/core';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
    selector: 'app-profile-avatar',
    templateUrl: './profile-avatar.component.html',
    styleUrls: ['./profile-avatar.component.scss'],
    standalone: true,
    imports: [
        MatTooltipModule
    ]
})
export class ProfileAvatarComponent implements OnInit {
    @Input() profileImagePath: Nullable<string>;
    @Input() firstName = '';
    @Input() lastName = '';
    @Input() initialsFontSizePx = 20;
    @Input() avatarSizePx = 60;

    initials!: string;

    ngOnInit(): void {
        const firstLetterFirstName = this.firstName?.trim()?.at(0) ?? '';
        const firstLetterLastName = this.lastName?.trim()?.at(0) ?? '';

        this.initials = `${firstLetterFirstName}${firstLetterLastName}`;
    }
}
