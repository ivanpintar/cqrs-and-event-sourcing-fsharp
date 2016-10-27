import React from 'react';
import { FormGroup, ControlLabel, FormControl } from 'react-bootstrap'
import ModalDialog from '../../global/components/ModalDialog';

class AddModal extends React.Component{
    constructor(props) {
        super(props);
        this.handleNameChange = this.handleNameChange.bind(this);
        this.handlePriceChange = this.handlePriceChange.bind(this);
        this.submit = this.submit.bind(this);
        this.state = { 
            name: '',
            price: '',
            errors: {}
        }; 
    }

    handleNameChange(e) {
        this.setState({ name: e.target.value });
    }

    handlePriceChange(e) {
        this.setState({ price: e.target.value });
    }

    submit() {        
        if(this.state.name && this.state.price >= 0 && this.state.price !== '') {
            this.props.addProduct(this.state.name, this.state.price);
            this.setState({ name: '', price: '', errors:{} })
            this.props.onClose();
            return;
        }

        let errors = {};

        if(!this.state.name) errors.name = true;
        if(this.state.price < 0 || this.state.price === '') errors.price = true;

        this.setState({errors: errors});
    }

    render() {
        let nameValState = null;
        let priceValState = null;
        if(this.state.errors.name) nameValState = 'error';
        if(this.state.errors.price) priceValState = 'error';

        return (
            <ModalDialog 
                show={this.props.show}
                onClose={this.props.onClose}
                onSave={this.submit}
                title='Add Product'>
                <FormGroup validationState={nameValState}>
                    <ControlLabel>Name</ControlLabel>
                    <FormControl 
                            value={this.state.name}
                            type='text'
                            onChange={this.handleNameChange}/>
                </FormGroup>
                <FormGroup validationState={priceValState}>
                    <ControlLabel>Price</ControlLabel>
                    <FormControl 
                        value={this.state.price}
                        type='number' 
                        onChange={this.handlePriceChange}/>
                </FormGroup>
            </ModalDialog>
        );
    }
}

export default AddModal;