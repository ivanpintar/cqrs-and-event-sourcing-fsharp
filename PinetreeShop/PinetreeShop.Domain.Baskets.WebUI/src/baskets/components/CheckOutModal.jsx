import React from 'react';
import { FormControl, FormGroup, ControlLabel } from 'react-bootstrap'
import ModalDialog from '../../global/components/ModalDialog';

class CheckOutModal extends React.Component{
    constructor(props) {
        super(props);
        this.handleChange = this.handleChange.bind(this);
        this.submit = this.submit.bind(this);
        this.close = this.close.bind(this);
        this.state = {
            streetAndNumber: '',
            zipAndCity: '',
            stateOrProvince: '',
            country: ''
        }; 
    }

    handleChange(e, field) {
        var update = {};
        update[field] = e.target.value;
        this.setState(update);
    }

    close() {
        this.props.onClose();
    }

    submit() {    
        if(
            this.state.streetAndNumber &&
            this.state.zipAndCity &&
            this.state.stateOrProvince &&
            this.state.country
        ) {
            this.props.checkOutBasket({
                streetAndNumber: this.state.streetAndNumber,
                zipAndCity: this.state.zipAndCity,
                stateOrProvince: this.stateOrProvince,
                country: this.country
            });
        }
        this.props.onClose();    
    }

    render() {
        return (
            <ModalDialog 
                show={this.props.show}
                onClose={this.props.onClose}
                onSave={this.submit}
                title='Check Out'>
				<FormGroup>
                    <ControlLabel>Street and number</ControlLabel>
					<FormControl
						value={this.state.streetAndNumber}
						type="text"
						onChange={(e) => this.handleChange(e, 'streetAndNumber')}/>
                </FormGroup>
				<FormGroup>
                    <ControlLabel>Zip and City</ControlLabel>
					<FormControl
						value={this.state.zipAndCity}
						type="text"
						onChange={(e) => this.handleChange(e, 'zipAndCity')}/>
                </FormGroup>
				<FormGroup>
                    <ControlLabel>State / Province</ControlLabel>
					<FormControl
						value={this.state.stateOrProvince}
						type="text"
						onChange={(e) => this.handleChange(e, 'stateOrProvince')}/>
                </FormGroup>
				<FormGroup>
                    <ControlLabel>Country</ControlLabel>
					<FormControl
						value={this.state.country}
						type="text"
						onChange={(e) => this.handleChange(e, 'country')}/>
                </FormGroup>
            </ModalDialog>
        );
    }
}

export default CheckOutModal;